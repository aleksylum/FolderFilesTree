using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FolderFilesTree;

namespace FolderFilesTree
{

    public class Reader : IDisposable
    {
        private readonly Int64 _blockMaxSize;
        private readonly BlockingCollection<ComponentForHash> _componentsForHash;
        private readonly Writer _writer;
        private readonly Thread _readerThread;
        private readonly CancellationToken _token;

        public Reader(Writer writer, Int64 blockMaxSize, CancellationToken token, Int32 queueMaxSize)
        {
            _writer = writer;
            _blockMaxSize = blockMaxSize;
            _token = token;
            _componentsForHash = new BlockingCollection<ComponentForHash>(queueMaxSize);
            _readerThread = CreateThread();
        }


        public void AddToQueue(ComponentForHash component)
        {
            Boolean b;
            do
            {
                b = _componentsForHash.TryAdd(component);

            } while (!b);

        }

        public void SignalAboutAddingCompleted()
        {
            _componentsForHash.CompleteAdding();
        }

        private Thread CreateThread()
        {
            Thread hashThread = new Thread(ReadInCycle);
            hashThread.Name = "Reader";
            hashThread.IsBackground = true;
            hashThread.Start();
            return hashThread;
        }

        private void ReadInCycle()
        {
            try
            {
                while (!_componentsForHash.IsCompleted)
                {
                    TryTakeFromQueue();
                }
            }

            finally
            {
                _writer.SignalAboutAddingCompleted();
            }

        }

        private void TryTakeFromQueue()
        {
            try
            {
                TakeFromQueue();
            }
            catch (Exception e)//обрабатываю здесь, чтобы не потерять поток при исключении
            {
                IExceptionHandler h = new ExceptionHandler();
                h.HandleException(e);
            }

        }

        private void TakeFromQueue()
        {
            ComponentForHash component;
            if (_componentsForHash.TryTake(out component, -1, _token))
            {
                TrySeparate(component);
            }
        }

        private void TrySeparate(ComponentForHash component)
        {
            try
            {
                Separate(component);
            }
            catch//если произошла ошибка во время чтения --
                 //результат null и хэш не будет рассчитываться дальше
            {
                component.Finished.Reset(0);
                component.OurFileReference.WriteHashCode(null);
                throw;
            }
        }


        private void Separate(ComponentForHash component)
        {
            String fileName = component.OurFileReference.Name;
            using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ReadPartsOfFile(stream, component);
            }
        }

        private void ReadPartsOfFile(Stream stream, ComponentForHash component)
        {
            for (int i = 0; i < component.NumberOfFileParts; ++i)
            {

                TryReadPart(stream, component, i);
            }

        }


        private void TryReadPart(Stream stream, ComponentForHash component, Int32 index)
        {
            stream.Seek(index * _blockMaxSize, SeekOrigin.Begin);
            byte[] buffer = new byte[_blockMaxSize];
            stream.Read(buffer, 0, buffer.Length);
            AddPartOfFileToWriter(buffer, component);
        }

        private void AddPartOfFileToWriter(byte[] buffer, ComponentForHash component)
        {
            PartOfFileForHash partOfFile = new PartOfFileForHash(buffer, component);
            _writer.AddToQueue(partOfFile);
        }

        public void Dispose()
        {
            _componentsForHash?.Dispose();
        }
    }
}
