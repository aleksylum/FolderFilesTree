using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FolderFilesTree
{
    public static class TreePrinter
    {
        public static void Print(FolderTree folderTree)
        {
            Stack<TreeNode> dirStackForPrintTree = new Stack<TreeNode>();
            dirStackForPrintTree.Push(folderTree.Tree[0]);
            PrintTree(dirStackForPrintTree);
        }

        private static void PrintTree(Stack<TreeNode> dirStackForPrintTree)
        {

            while (dirStackForPrintTree.Count != 0)
            {

                TreeNode parentNode = dirStackForPrintTree.Pop();

                PrintFolderInformation(parentNode);

                AddFoldersInDirStack(dirStackForPrintTree, parentNode);

            }
        }

        private static void PrintFolderInformation(TreeNode parentNode)
        {
            Console.WriteLine($"\nFOLDER: {parentNode.Name}");

            PrintReparsePoints(parentNode);

            PrintFiles(parentNode);
        }

        private static void AddFoldersInDirStack(Stack<TreeNode> dirStackForPrintTree, TreeNode parentNode)
        {
            foreach (TreeNode d in parentNode.Children)
            {
                dirStackForPrintTree.Push(d);
            }
        }

        private static void PrintReparsePoints(TreeNode parentNode)
        {
            foreach (ReparsePoint r in parentNode.ReparsePoints)
            {
                Console.WriteLine($"\nREPARSE POINT: {r.Name}");
            }

        }

        private static void PrintFiles(TreeNode parentNode)
        {
            foreach (OurFile f in parentNode.Files)
            {
                Console.WriteLine($"\nFILE: {f.Name} {f.IsShortcut} {f.CreationTime} {f.LastWriteTime}\n" +
                                  $"FILE`S HASHCODE: {(f.HashCode != null ? (BitConverter.ToString(f.HashCode)) : " - - - ")}");
            }
        }

    }
}
