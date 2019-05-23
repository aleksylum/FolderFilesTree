using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FolderFilesTree
{

    public class ComponentCreator : IDisposable
    {
        private readonly Int64 _blockMaxSize;
        private readonly BlockingCollection<OurFile> _ourFiles;
        private readonly Thread _componentThread;
        private readonly Writer _writer;
        private readonly Reader _reader;
        private readonly CancellationToken _token;

        public ComponentCreator(CountdownEvent everythingIsFinished, CancellationToken token, Int32 queueMaxSize, Int64 blockMaxSize)
        {
            _ourFiles = new BlockingCollection<OurFile>(queueMaxSize);
            _componentThread = CreateCreatorThread();
            _blockMaxSize = blockMaxSize;
            _writer = new Writer(everythingIsFinished, token, queueMaxSize);
            _reader = new Reader(_writer, _blockMaxSize, token, queueMaxSize);
            _token = token;
        }


        public void PutOnQueue(OurFile ourFile)
        {
            Boolean b;
            do
            {
                b = _ourFiles.TryAdd(ourFile);
            } while (!b);
        }


        public void SignalAboutAddingCompleted()
        {
            _ourFiles.CompleteAdding();
        }


        private Thread CreateCreatorThread()
        {
            Thread hashThread = new Thread(CreateComponentInCycle);
            hashThread.Name = "Creator";
            hashThread.IsBackground = true;
            hashThread.Start();
            return hashThread;
        }


        

        private void CreateComponentInCycle()
        {
            try
            {
                while (!_ourFiles.IsCompleted)
                {

                    TryTakeFromQueue();
                }
            }

            finally
            {
                _reader.SignalAboutAddingCompleted();
            }

        }


        private void TryTakeFromQueue()
        {
            try
            {
                TakeFileFromQueue();
            }
            catch (Exception e)//обрабатываю здесь, чтобы не потерять поток при исключении
            {
                IExceptionHandler h = new ExceptionHandler();
                h.HandleException(e);

            }

        }

        private void TakeFileFromQueue()
        {

            OurFile currentFile;

            if (_ourFiles.TryTake(out currentFile, -1, _token))
            {
                TryCheckFileReadAccess(currentFile);
            }

        }

        private void TryCheckFileReadAccess(OurFile currentFile)
        {
            try
            {
                CheckFileReadAccess(currentFile);
            }
            catch
            {
                currentFile.WriteHashCode(null);
                throw;
            }

        }

        private void CheckFileReadAccess(OurFile currentFile)
        {
            using (var stream = File.Open(currentFile.Name, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                CreateNewComponentForReader(currentFile);
            }
        }

        private void CreateNewComponentForReader(OurFile ourFile)
        {
            Int32 numberOfFileParts = GetNumberOfFileParts(ourFile);
            CountdownEvent ourFileEvent = new CountdownEvent(numberOfFileParts);
            AddToReaderQueue(numberOfFileParts, ourFile, ourFileEvent);

        }

        private void AddToReaderQueue(Int32 numberOfFileParts, OurFile ourFile, CountdownEvent ourFileEvent)
        {
            ComponentForHash component = new ComponentForHash(numberOfFileParts, ourFile, ourFileEvent, null);
            _reader.AddToQueue(component);

        }

        private Int32 GetNumberOfFileParts(OurFile ourFile)
        {
            FileInfo fileInfo = new FileInfo(ourFile.Name);
            Int64 ourFileLength = fileInfo.Length;
            return (Int32)((ourFileLength + _blockMaxSize - 1) / _blockMaxSize);
        }

        public void Dispose()
        {
            _ourFiles?.Dispose();
            _reader?.Dispose();
            _writer?.Dispose();

        }

    }
}