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
    public class SeparateHashFile
    {

        private static readonly Int64 blockMaxSize;
        private static readonly Int32 threadCount;

        private BitArray _resultHash;
        private String _fileName;
        private Int64 _blockCount;
        private CountdownEvent _finished;
        private Object _locker;
        private BlockingCollection<byte[]> _collection;
        private Thread[] _threads;
        private Int32 _realThreadCount;

        static SeparateHashFile()
        {
            blockMaxSize = 16384;
            threadCount = Environment.ProcessorCount;// - 2
        }

        public SeparateHashFile(String name)
        {

            _locker = new Object();
            _fileName = name;
            _blockCount = (new FileInfo(_fileName).Length + blockMaxSize - 1) / blockMaxSize;
            _realThreadCount = (int)((threadCount > _blockCount) ? _blockCount : threadCount);
            _collection = new BlockingCollection<byte[]>(_realThreadCount);
            _threads = new Thread[_realThreadCount];
        }

        public Byte[] ParallelCalcResult()
        {
            _finished = new CountdownEvent((Int32)_blockCount);
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
                            stream.Seek(i * blockMaxSize, SeekOrigin.Begin);
                            byte[] buffer = new byte[blockMaxSize];

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

        public void EmergencyStopParallelWork()//если произошла внештатная ситуация во время обработки файла
        {
            for (int i = 0; i < _realThreadCount; ++i)
            {
                _threads[i].Abort();
            }
            _resultHash = null;
            _finished.Reset(0);
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