using System;
using System.Collections.Concurrent;
using System.IO;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading;

namespace FolderFilesTree
{
    public class FolderTree
    {
        public List<TreeNode> Tree { get; }


        public FolderTree()
        {
            Tree = new List<TreeNode>();
        }

        public void PrintTree()
        {
            TreePrinter.Print(this);
        }

        public FolderTree BuildFolderTree(String path)
        {
            using (TreeBuilder treeBuilder = new TreeBuilder())
            {
                return treeBuilder.Build(path, this);
            }

        }

    }
}