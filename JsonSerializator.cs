using System.IO;
using System.Runtime.Serialization.Json;

namespace FolderFilesTree
{
    class JsonSerializator : ISerializator
    {
        private DataContractJsonSerializer jf = new DataContractJsonSerializer(typeof(TreeNode));
        private string path = "tree.xml";

        public void SerializeTree(FolderTree folderTree)
        {
        
            using (FileStream fs = File.Create(path))
            {
                jf.WriteObject(fs, folderTree.Tree[0]);
            }
        }

        public FolderTree DeserializeTree()
        {
            FolderTree folderTree = new FolderTree();
            
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                folderTree.Tree.Add((TreeNode)jf.ReadObject(fs));
            }
            return folderTree;
        }
    }
}

