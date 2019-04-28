using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderFilesTree
{
    public static class ArrayConverter
    {
        public static Byte[] ConvertBitsToBytes(BitArray bitArray)
        {
            if (bitArray != null)
            {
                Byte[] hashBytes = new Byte[(bitArray.Length - 1) / 8 + 1];
                bitArray.CopyTo(hashBytes, 0);
                return hashBytes;
            }
            return null;
        }
    }
}
