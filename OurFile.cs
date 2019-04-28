using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;


namespace FolderFilesTree
{
    [Serializable]
    public class OurFile
    {
        public String Name { get; }
        public DateTime CreationTime { get; }
        public DateTime LastWriteTime { get; }
        public bool IsShortcut { get; }
        public Byte[] HashCode { get; set; }

        public OurFile() { }

        private OurFile(String name, DateTime creationTime, DateTime lastWriteTime, bool isShortcut, Byte[] hashCode)
        {
            Name = name;
            CreationTime = creationTime;
            LastWriteTime = lastWriteTime;
            IsShortcut = isShortcut;
            HashCode = hashCode;
        }

        public static OurFile Create(String name)
        {
            FileInfo fi = new FileInfo(name);
            bool isShortcut = (Path.GetExtension(name) == ".lnk");
            OurFile ourFile = new OurFile(name, fi.CreationTime, fi.LastWriteTime, isShortcut, null);
            if (fi.Length != 0)
            {
               HashCreator.PutOnHashQueue(ourFile);
            }       
            return ourFile;
        }

    }

}
