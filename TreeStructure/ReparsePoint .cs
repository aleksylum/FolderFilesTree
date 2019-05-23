using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FolderFilesTree
{
    [Serializable]
    public class ReparsePoint
    {

        public String Name { get; }


        public ReparsePoint(string name)
        {
            Name = name;
        }

    }
}
