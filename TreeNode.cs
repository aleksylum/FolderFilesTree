using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FolderFilesTree
{
    [Serializable]
    [DataContract]
    public class TreeNode
    {
        [DataMember]
        public String Name { get; set; }
        [DataMember]
        public List<TreeNode> Children { get; set; }
        [DataMember]
        public List<OurFile> Files { get; set; }
        [DataMember]
        public List<ReparsePoint> ReparsePoints { get; set; }

        public TreeNode() { }
        public TreeNode(String name)
        {
            Name = name;
            Children = new List<TreeNode>();
            Files = new List<OurFile>();
            ReparsePoints = new List<ReparsePoint>();
        }

    }
}
