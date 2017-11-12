using System;
using System.Collections.Generic;
using System.IO;

using VFS.Core;

namespace VFS.FileSystem
{
    public class LocalFileTree : INodeTree
    {
        public Dictionary<int, Node> FlatTree { get { return mNodeTree; } }
        public bool IsReady { get; internal set; }

        public bool ReadOnly { get { return false; } }

        private Dictionary<int, Node> mNodeTree;
        private DirectoryInfo mRootPath;

        public LocalFileTree()
        {
            mNodeTree = new Dictionary<int, Node>();
            IsReady = false;
        }

        public bool Build(string root)
        {
            IsReady = false;
            mNodeTree.Clear();

            if (Directory.Exists(root))
            {
                mRootPath = new DirectoryInfo(root);
                BuildRecursive(root);

                IsReady = true;
            }

            return IsReady;
        }

        public bool DirExists(string path)
        {
            bool result = mNodeTree.TryGetValue(path.GetHashCode(), out Node node);

            return result && (node.NodeType == Node.INodeType.Directory);
        }

        public bool FileExists(string path)
        {
            bool result = mNodeTree.TryGetValue(path.GetHashCode(), out Node node);

            return result && (node.NodeType == Node.INodeType.File);
        }

        public Stream Open(string path)
        {
            FileStream result = null;
            if (FileExists(path))
            {
                result = File.Open(Path.Combine(mRootPath.FullName, path), FileMode.Open, FileAccess.Read);
            }
            return result;
        }

        public bool Delete(string path)
        {
            bool result = false;
            if (FileExists(path))
            {
                try
                {
                    var value = mNodeTree[path.GetHashCode()];
                    var fullNameOfFileToDelete = Path.Combine(mRootPath.FullName, value.Name);
                    File.Delete(fullNameOfFileToDelete);
                    mNodeTree.Remove(path.GetHashCode());
                    result = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to delete file: {0}", e.Message);
                }
            }

            return result;
        }

        public bool Write(string path, byte[] buffer, int length)
        {
            // does the directory exist?
            string dir = path.Substring(0, path.LastIndexOf('\\'));
            if (!DirExists(dir))
            {
                // We have to build the directory then write the file
                try
                {
                    DirectoryInfo info = Directory.CreateDirectory(Path.Combine(mRootPath.FullName, dir));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unable to create directory {0} {1}", dir, e.Message);
                    return false;
                }
            }

            // Go ahead and write
            using (FileStream fs = File.Open(Path.Combine(mRootPath.FullName, path), FileMode.CreateNew, FileAccess.Write))
            {
                fs.Write(buffer, 0, length);
            }

            mNodeTree.Add(path.GetHashCode(), new Node(path, Node.INodeType.File));

            return true;
        }

        protected void BuildRecursive(string root)
        {
            foreach (var path in Directory.EnumerateDirectories(root))
            {
                string truncatedPath = path.Substring(mRootPath.FullName.Length);
                mNodeTree.Add(truncatedPath.GetHashCode(), new Node(truncatedPath, Node.INodeType.Directory));
                BuildRecursive(path);
            }

            foreach (var file in Directory.EnumerateFiles(root))
            {
                string truncatedPath = file.Substring(mRootPath.FullName.Length);
                mNodeTree.Add(truncatedPath.GetHashCode(), new Node(truncatedPath, Node.INodeType.File));
            }
        }
    }
}
