using System.IO;
using System.Xml.Serialization;

namespace FolderFilesTree
{
    class XmlSerializator : ISerializator
    {
        private XmlSerializer xs = new XmlSerializer(typeof(TreeNode));
        private string path = "tree.xml";

        public void SerializeTree(FolderTree folderTree)
        {
            using (FileStream fs = File.Create(path))
            {
                xs.Serialize(fs, folderTree.Tree[0]);
            }
        }

        public FolderTree DeserializeTree()
        {
            FolderTree folderTree = new FolderTree();

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                folderTree.Tree.Add((TreeNode)xs.Deserialize(fs));
            }
            return folderTree;
        }
    }
}

