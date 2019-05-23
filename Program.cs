using System;


namespace FolderFilesTree
{
    public class Program
    {
        static void Main(string[] args)
        {
            ILogger log = Logger.GetLogger();
            log.LogStartTime();

            FolderTree tree = new FolderTree();
            tree.BuildFolderTree(@"C:\");

            ISerializator s = new JsonSerializator();
            //s = new BinSerializator();
            //s = new XmlSerializator();

            s.SerializeTree(tree);

            FolderTree tree2 = s.DeserializeTree();

            tree2.PrintTree();

            log.LogFinalTime();

            Console.ReadKey();
        }

    }
}