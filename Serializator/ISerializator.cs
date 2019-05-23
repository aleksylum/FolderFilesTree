namespace FolderFilesTree
{
     interface ISerializator
    {
        void SerializeTree(FolderTree folderTree);
        FolderTree DeserializeTree();
    }
}
