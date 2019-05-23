using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FolderFilesTree
{
    public static class Hasher
    {
        public static Byte[] CalcSHA(Byte[] blockBytes)
        {
            using (SHA256 mySHA256 = SHA256.Create())
            {
                return mySHA256.ComputeHash(blockBytes);
            }
        }
    }
}




//public static async Task<Byte[]> AsyncCalcHash(String fileName)
//{
//    return await Task<Byte[]>.Run(() => Hash.CalcSHA(fileName));
//}