using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FolderFilesTree
{
    public class Writer : IDisposable
    {
        private readonly BlockingCollection<PartOfFileForHash> _partOfFileForHash;
        private Thread[] _threads;
        private readonly CountdownEvent _everythingIsFinished;
        private readonly Object _locker;
        private readonly CancellationToken _token;

        public Writer(CountdownEvent everythingIsFinished, CancellationToken token, Int32 queueMaxSize)
        {
            _token = token;
            _everythingIsFinished = everythingIsFinished;
            _locker = new Object();
            _partOfFileForHash = new BlockingCollection<PartOfFileForHash>(queueMaxSize);

            StartParallelWork(_everythingIsFinished.InitialCount);

        }

        public void AddToQueue(PartOfFileForHash partOfFile)
        {
            Boolean b;
            do
            {
                b = _partOfFileForHash.TryAdd(partOfFile);//добавление извне (reader)

            } while (!b);

        }


        public void SignalAboutAddingCompleted()
        {
            _partOfFileForHash.CompleteAdding();
        }


        private void StartParallelWork(Int32 threadCount)
        {
            _threads = new Thread[threadCount];
            for (int i = 0; i < threadCount; ++i)
            {
                _threads[i] = new Thread(ParallelPartsOfFileProcessing);
                _threads[i].Name = $"Writer {i}";
                _threads[i].Start();
            }

        }


        private void ParallelPartsOfFileProcessing()
        {
            try
            {
                while (!_partOfFileForHash.IsCompleted)
                {
                    TryTakeFromQueue();
                }
            }

            finally
            {
                _everythingIsFinished.Signal();
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
            PartOfFileForHash part;

            if (_partOfFileForHash.TryTake(out part, -1, _token))
            {

                CheckIsNotFinised(part);
            }
        }

        private void PutResultHashIntoFile(PartOfFileForHash part)
        {
            if (part.Finished.IsSet)
            {
                Byte[] hashCode = ArrayConverter.ConvertBitsToBytes(part.HashCodeTemporaryResult);
                part.OurFileReference.WriteHashCode(hashCode);
            }
        }

        private void CheckIsNotFinised(PartOfFileForHash part)
        {
            if (!part.Finished.IsSet)
            //проверка на случай, если сбросили ивент из ридера при ошибки чтениия на середине файла(не разу не возникала)
            {
                TryCalcPartOfHash(part);
                PutResultHashIntoFile(part);
            }

        }

        private void TryCalcPartOfHash(PartOfFileForHash part)
        {
            try
            {
                CalcPartOfHash(part);

            }
            finally
            {
                part.Finished.Signal();
            }

        }

        private void CalcPartOfHash(PartOfFileForHash part)
        {
            byte[] hashBytes = Hasher.CalcSHA(part.PartOfFile);
            lock (_locker)
            {
                XorHash(hashBytes, part);
            }

        }

        private void XorHash(byte[] hashBytes, PartOfFileForHash part)
        {

            if (part.HashCodeTemporaryResult != null)
            {
                BitArray bits = new BitArray(hashBytes);

                part.HashCodeTemporaryResult = part.HashCodeTemporaryResult.Xor(bits);
            }
            else
            {
                part.HashCodeTemporaryResult = new BitArray(hashBytes);
            }

        }

        public void Dispose()
        {
            _partOfFileForHash?.Dispose();
        }

    }
}
