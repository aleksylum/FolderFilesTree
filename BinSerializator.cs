using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace FolderFilesTree
{
   class BinSerializator : ISerializator
    {

        private BinaryFormatter bf = new BinaryFormatter();
        private string path = "tree.bin";

        public void SerializeTree(FolderTree folderTree)
        {
            using (FileStream fs = File.Create(path))
            {
                bf.Serialize(fs,folderTree.Tree[0]);
            }       
        }

        public FolderTree DeserializeTree()
        {
            FolderTree folderTree = new FolderTree();
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                folderTree.Tree.Add((TreeNode)bf.Deserialize(fs));
            }
            return folderTree;
        }
    }
}
