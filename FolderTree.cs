using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Permissions;


namespace FolderFilesTree
{
    public class FolderTree
    {
        public List<TreeNode> Tree;
        public FolderTree() { Tree = new List<TreeNode>(); }
        public FolderTree(string root)
        {
            Tree = new List<TreeNode>() { new TreeNode(root) };

            Queue<TreeNode> dirTree = new Queue<TreeNode>();

            //корень дерева

            dirTree.Enqueue(Tree[0]);//
            while (dirTree.Count != 0)
            {
                //достаем родит узел и удаляем из очереди
                TreeNode parentNode = dirTree.Dequeue();

                try
                {
                    //if (parentNode.Name == ($"C:\\Windows\\CSC\\v2.0.6"))
                    //{
                    //}

                    if (CheckFolderAccess.CheckAccess(parentNode.Name))
                    {
                        string[] tempDir = Directory.GetDirectories(parentNode.Name);
                        //массив поддиректорий(строк, а не узлов)
                        foreach (string d in tempDir)
                        {
                            //создаем узел
                            TreeNode currNode = new TreeNode(d);
                            Tree.Add(currNode);

                            dirTree.Enqueue(currNode);

                            //добавляем информацию родителю о дочерних подпапках
                            parentNode.Children.Add(currNode);
                        }

                        string[] tempFiles = Directory.GetFiles(parentNode.Name);

                        foreach (string f in tempFiles)
                        {
                            parentNode.Files.Add(new OurFile(f));
                        }

                    }

                    else
                    {
                        //  ILogger logger = Logger.GetLogger();
                        // logger.LogAccessDenied(parentNode.Name);
                    }
                }
                catch (Exception e)
                {
                    IExceptionHandler h = new ExceptionHandler();
                    h.HandleException(e);
                }
            }
        }



        public void PrintTree()
        {
            Stack<TreeNode> dirTree = new Stack<TreeNode>();
            dirTree.Push(Tree[0]);

            while (dirTree.Count != 0)
            {
                //достаем родит узел
                TreeNode parentNode = dirTree.Pop();
                Console.WriteLine("FOLDER: " + parentNode.Name);

                foreach (OurFile f in parentNode.Files)
                {
                    Console.WriteLine($"FILE:   {f.Name} {f.IsShortcut} {f.CreationTime} {f.LastWriteTime}");
                }

                foreach (TreeNode d in parentNode.Children)
                {
                    dirTree.Push(d);
                }
            }
            Console.ReadKey();
        }

    }
}