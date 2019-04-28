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

    public static class HashCreator
    {
        private static readonly BlockingCollection<OurFile> _ourFilesForGetHash;
        private static readonly Thread _hashThread;

        static HashCreator()
        {
            _ourFilesForGetHash = new BlockingCollection<OurFile>(30);
            _hashThread = new Thread(GetResultHash);
            _hashThread.Name = "HashThread";
            _hashThread.Start();
        }

        public static void PutOnHashQueue(OurFile ourFile)
        {
            _ourFilesForGetHash.Add(ourFile);
        }


        public static void GetResultHash()
        {
            while (true)
            {
                OurFile currentFile;
                if (_ourFilesForGetHash.TryTake(out currentFile))
                {
                    try
                    {
                        using (var stream = File.Open(currentFile.Name, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            SeparateHashFile separateHashFile = new SeparateHashFile(currentFile.Name);
                            currentFile.HashCode = separateHashFile.ParallelCalcResult();
                        }
                    }
                    catch (IOException e)
                    {
                        {
                            IExceptionHandler h = new ExceptionHandler();
                            h.HandleException(e);

                        }

                    }
                }

            }

        }
    }
}
