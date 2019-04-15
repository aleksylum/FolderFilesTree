using System;


namespace FolderFilesTree
{
    public class Program
    {
        static void Main(string[] args)
        {
            ILogger log = Logger.GetLogger();
            log.LogStartTime();

            FolderTree tree = new FolderTree(@"C:\");

            ISerializator s;
            //s = new BinSerializator();
            //s = new XmlSerializator();
            s = new JsonSerializator();

            s.SerializeTree(tree);
            FolderTree tree2 = s.DeserializeTree();

            log.LogFinalTime();

            tree2.PrintTree();
            Console.ReadKey();
        }

    }
}