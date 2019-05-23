using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FolderFilesTree
{
    public class TreeBuilder : IDisposable
    {
        private readonly ComponentCreator _componentCreator;

        public readonly CountdownEvent _everythingIsFinished;

        public readonly CancellationTokenSource _tokenSource;

        private readonly Queue<TreeNode> _dirQueueForTree;


        public TreeBuilder()
        {
            _everythingIsFinished = new CountdownEvent(Environment.ProcessorCount);
            _tokenSource = new CancellationTokenSource();
            _componentCreator = new ComponentCreator(_everythingIsFinished, _tokenSource.Token, 100, 1677721);
            _dirQueueForTree = new Queue<TreeNode>();

        }


        public FolderTree Build(String root, FolderTree tree)
        {

            _dirQueueForTree.Enqueue(new TreeNode(root));

            while (_dirQueueForTree.Count != 0)
            {
                AddNodeIntoTree(tree);
            }

            CompleteBuild();
            return tree;
        }


        private void AddNodeIntoTree(FolderTree tree)
        {

            TreeNode parentNode = _dirQueueForTree.Dequeue();

            try
            {
                AddFolderInformation(parentNode);
            }
            catch (Exception e)
            {
                IExceptionHandler h = new ExceptionHandler();
                h.HandleException(e);
            }

            tree.Tree.Add(parentNode);
        }


        private void AddFolderInformation(TreeNode parentNode)
        {
            if (AccessChecker.Check(parentNode.Name))
            {
                AddDirectories(parentNode);
                AddFiles(parentNode);

            }
            else
            {
                ILogger logger = Logger.GetLogger();
                logger.LogAccessDenied(parentNode.Name);
            }
        }

        private void AddFiles(TreeNode parentNode)
        {
            string[] tempFiles = Directory.GetFiles(parentNode.Name);

            foreach (string f in tempFiles)
            {
                parentNode.Files.Add(OurFile.Create(f, _componentCreator));
            }
        }


        private void AddDirectories(TreeNode parentNode)
        {
            DirectoryInfo dir = new DirectoryInfo(parentNode.Name);
            IEnumerable<DirectoryInfo> diArr = dir.GetDirectories();

            foreach (DirectoryInfo d in diArr)
            {
                if (!d.Attributes.HasFlag(FileAttributes.ReparsePoint))
                {

                    AddDirectory(d, parentNode);
                }
                else
                {
                    AddReparsePoin(d, parentNode);
                }
            }
        }

        private void AddReparsePoin(DirectoryInfo d, TreeNode parentNode)
        {
            ReparsePoint rp = new ReparsePoint(d.FullName);
            parentNode.ReparsePoints.Add(rp);
        }


        private void AddDirectory(DirectoryInfo d, TreeNode parentNode)
        {
            TreeNode currNode = new TreeNode(d.FullName);
            _dirQueueForTree.Enqueue(currNode);
            parentNode.Children.Add(currNode);
        }


        private void CompleteBuild()
        {
            _componentCreator.SignalAboutAddingCompleted();
            _everythingIsFinished.Wait();
            Cancel();
        }


        public void Cancel()
        {
            _tokenSource.Cancel();
        }

        public void Dispose()
        {
            _everythingIsFinished?.Dispose();
            _tokenSource?.Dispose();
            _componentCreator?.Dispose();
        }
    }

}

