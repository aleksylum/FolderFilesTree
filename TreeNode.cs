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
        public string Name { get; set; }
        [DataMember]
        public List<TreeNode> Children { get; set; }
        [DataMember]
        public List<OurFile> Files { get; set; }

        public TreeNode() { }
        public TreeNode(string name)
        {
            Name = name;
            Children = new List<TreeNode>();
            Files = new List<OurFile>();
        }

    }
}
