using System;
using System.IO;


namespace FolderFilesTree
{
    [Serializable]
    public class OurFile
    {
        public string Name { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public bool IsShortcut { get; set; }

        public OurFile() { }

        public OurFile(string name)
        {
            Name = name;
            FileInfo fi = new FileInfo(name);
            CreationTime = fi.CreationTime;
            LastWriteTime = fi.LastWriteTime;

            if (Path.GetExtension(name) == ".lnk")
                IsShortcut = true;
            else
                IsShortcut = false;
        }
    }
}
