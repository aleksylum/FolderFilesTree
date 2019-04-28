using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text.RegularExpressions;

namespace FolderFilesTree
{
    public class FolderTree
    {
        public List<TreeNode> Tree;
        
        public FolderTree()
        {
            Tree = new List<TreeNode>();// { new TreeNode(root) };

        }
        public static FolderTree Create(string root)
        {

            FolderTree folderTree = new FolderTree();
            //корень дерева
            Queue<TreeNode> dirTree = new Queue<TreeNode>();
            dirTree.Enqueue(new TreeNode(root));//
            while (dirTree.Count != 0)
            {
                //достаем родит узел и удаляем из очереди
                TreeNode parentNode = dirTree.Dequeue();

                try
                {
                    //if (parentNode.Name == ($"C:\\Users\\shasha\\Test\\Link"))//C:\\Users\\shasha\\AppData
                    //{
                    //    Console.WriteLine("!!!");
                    //}

                    //if (parentNode.Name == ($"C:\\Users\\shasha\\Test\\Link\\Link"))//C:\\Users\\shasha\\AppData
                    //{
                    //    Console.WriteLine("!!!");
                    //}

                    if (CheckFolderAccess.CheckAccess(parentNode.Name))
                    {
                        //string[] tempDir = Directory.GetDirectories(parentNode.Name);
                        //массив поддиректорий(строк, а не узлов)
                        DirectoryInfo dir = new DirectoryInfo(parentNode.Name);
                        IEnumerable<DirectoryInfo> diArr = dir.GetDirectories();

                        foreach (DirectoryInfo d in diArr)
                        {
                            if (!d.Attributes.HasFlag(FileAttributes.ReparsePoint))
                            {
                                //создаем узел
                                TreeNode currNode = new TreeNode(d.FullName);

                                dirTree.Enqueue(currNode);
                                //добавляем информацию родителю о дочерних подпапках
                                parentNode.Children.Add(currNode);
                            }
                            else
                            {
                                parentNode.ReparsePoints.Add(new ReparsePoint(d.FullName));
                            }
                        }

                        string[] tempFiles = Directory.GetFiles(parentNode.Name);

                        foreach (string f in tempFiles)
                        {
                            parentNode.Files.Add(OurFile.Create(f));
                        }

                    }
                    else
                    {
                        ILogger logger = Logger.GetLogger();
                        logger.LogAccessDenied(parentNode.Name);
                    }
                }
                catch (Exception e)
                {
                    IExceptionHandler h = new ExceptionHandler();
                    h.HandleException(e);
                }

                folderTree.Tree.Add(parentNode);
            }
            return folderTree;
        }



        public void PrintTree()
        {
            Stack<TreeNode> dirTree = new Stack<TreeNode>();
            dirTree.Push(Tree[0]);

            while (dirTree.Count != 0)
            {
                //достаем родит узел
                TreeNode parentNode = dirTree.Pop();
                Console.WriteLine($"\nFOLDER: {parentNode.Name}");

                foreach (ReparsePoint r in parentNode.ReparsePoints)
                {
                    Console.WriteLine($"REPARSE POINT: {r.Name}");
                }

                foreach (OurFile f in parentNode.Files)
                {
                    Console.WriteLine($"FILE: {f.Name} {f.IsShortcut} {f.CreationTime} {f.LastWriteTime}\n" +
                        $"FILE`S HASHCODE: {(f.HashCode != null ? (BitConverter.ToString(f.HashCode)) : " - - - ")}");
                }
                          
                foreach (TreeNode d in parentNode.Children)
                {
                    dirTree.Push(d);
                }
            }
        }

    }
}