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
        private bool _wasHashCodeCalculate;

        private Byte[] _hashCode;
        public Byte[] HashCode
        {
            get
            {
                if (_wasHashCodeCalculate) return _hashCode;
                else throw new FieldAccessException();//хэш ещё считается
            }

        }


        public OurFile() { }


        private OurFile(String name, DateTime creationTime, DateTime lastWriteTime, bool isShortcut)
        {
            Name = name;
            CreationTime = creationTime;
            LastWriteTime = lastWriteTime;
            IsShortcut = isShortcut;
            _wasHashCodeCalculate = false;
        }


        public static OurFile Create(String name, ComponentCreator componentCreator)
        {
            FileInfo fi = new FileInfo(name);
            bool isShortcut = (Path.GetExtension(name) == ".lnk");
            OurFile ourFile = new OurFile(name, fi.CreationTime, fi.LastWriteTime, isShortcut);

            if (fi.Length != 0)
            {
                componentCreator.PutOnQueue(ourFile);
            }
            else ourFile.WriteHashCode(new Byte[0]);

            return ourFile;
        }

        public void WriteHashCode(Byte[] arr)
        {
            _hashCode = arr;
            _wasHashCodeCalculate = true;
        }
    }
}