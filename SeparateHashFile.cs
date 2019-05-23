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
    public class SeparateHashFile : IDisposable
    {

     
        

        private BitArray _resultHash;
        private String _fileName;
        private Int32 _blockCount;
        private Int32 _realThreadCount;
        private CountdownEvent _finished;
        
        private BlockingCollection<byte[]> _collection;
   



        public SeparateHashFile(String name)
        {
           
            _fileName = name;
            _blockCount = (Int32)((new FileInfo(_fileName).Length + _blockMaxSize - 1) / _blockMaxSize);
            _realThreadCount = (_threadCount > _blockCount) ? _blockCount : _threadCount;
            _collection = new BlockingCollection<byte[]>(_realThreadCount);
            _threads = new Thread[_realThreadCount];
            _finished = new CountdownEvent(_blockCount);
        }

        public Byte[] ParallelCalcResult()
        {
            Separate();
            _finished.Wait();
            return ArrayConverter.ConvertBitsToBytes(_resultHash);
        }


       

       

        public void StartParallelWork()
        {
            for (int i = 0; i < _realThreadCount; ++i)
            {
                _threads[i] = new Thread(ParallelGetHash);
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

 

        public void Dispose()
        {
            _finished.Dispose();
            _collection.Dispose();
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