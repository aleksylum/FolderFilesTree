using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FolderFilesTree
{
    public class SeparateHashFile: IDisposable
    {

        private static readonly Int64 _blockMaxSize;
        private static readonly Int32 _threadCount;

        private BitArray _resultHash;
        private String _fileName;
        private Int32 _blockCount;
        private Int32 _realThreadCount;
        private CountdownEvent _finished;
        private Object _locker;
        private BlockingCollection<byte[]> _collection;
        private Thread[] _threads;


        static SeparateHashFile()
        {
            _blockMaxSize = 16384;
            _threadCount = Environment.ProcessorCount - 2;
        }

        public SeparateHashFile(String name)
        {
            _locker = new Object();
            _fileName = name;
            _blockCount = (Int32)((new FileInfo(_fileName).Length + _blockMaxSize - 1) / _blockMaxSize);
            _realThreadCount = (_threadCount > _blockCount) ? _blockCount : _threadCount;
            _collection = new BlockingCollection<byte[]>(_realThreadCount);
            _threads = new Thread[_realThreadCount];
        }

        public Byte[] ParallelCalcResult()
        {
            _finished = new CountdownEvent(_blockCount);
            Separate();
            _finished.Wait();
            return ArrayConverter.ConvertBitsToBytes(_resultHash);
        }


        public void Separate()
        {
            try
            {
                using (var stream = new FileStream(_fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    StartParallelWork();

                    try
                    {
                        for (int i = 0; i < _blockCount; ++i)
                        {
                            stream.Seek(i * _blockMaxSize, SeekOrigin.Begin);
                            byte[] buffer = new byte[_blockMaxSize];
                            stream.Read(buffer, 0, buffer.Length);
                            _collection.Add(buffer);

                        }

                        _collection.CompleteAdding();
                    }
                    catch (Exception e)
                    {
                        EmergencyStopParallelWork();
                        throw e;
                    }
                }
            }
            catch (Exception e)
            {
                IExceptionHandler h = new ExceptionHandler();
                h.HandleException(e);
                _resultHash = null;
                _finished.Reset(0);

            }
        }
        public void StartParallelWork()
        {
            for (int i = 0; i < _realThreadCount; ++i)
            {
                _threads[i] = new Thread(this.ParalellGetHash);
                _threads[i].Name = $"{i} {_fileName}";
                _threads[i].Start();
            }

        }

        public void EmergencyStopParallelWork()
        //если произошла внештатная ситуация во время обработки файла
        //(ни разу не использовалась)
        {
            for (int i = 0; i < _realThreadCount; ++i)
            {
                _threads[i].Abort();
            }

        }

        public void ParalellGetHash()
        {
            while (!_collection.IsCompleted)//!finished.IsSet
            {
                try
                {
                    byte[] bytes;

                    if (_collection.TryTake(out bytes))//new TimeSpan(0, 0, 3)
                    {
                        byte[] hashBytes = Hasher.CalcSHA(bytes);
                        lock (_locker)
                        {
                            if (_resultHash != null)
                            {
                                BitArray bits = new BitArray(hashBytes);

                                _resultHash = _resultHash.Xor(bits);
                            }
                            else
                            {
                                _resultHash = new BitArray(hashBytes);
                            }
                        }
                        _finished.Signal();
                    }

                }
                catch (Exception e)
                {
                    IExceptionHandler h = new ExceptionHandler();
                    h.HandleException(e);
                    _finished.Signal();
                }

            }

        }

        public void Dispose()
        {
            _finished.Dispose();
        }
    }
}


//if (actualLength != buffer.Length)
//{
//    buffer = SubArray(buffer, 0, actualLength);
//}


//public Byte[] SubArray(Byte[] data, int index, int length)
//{
//    Byte[] result = new Byte[length];
//    Array.Copy(data, index, result, 0, length);
//    return result;
//}