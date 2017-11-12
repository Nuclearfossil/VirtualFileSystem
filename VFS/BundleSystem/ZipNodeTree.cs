using System.Collections.Generic;

using SharpCompress.Archives.Zip;

using VFS.Core;
using System.IO;

namespace VFS.BundleSystem
{
    public class ZipNode : Node
    {
        public ZipNode(string path, INodeType nodeType) : base(path, nodeType)
        {
            mArchiveEntry = null;
        }

        public ZipNode(string path, INodeType nodeType, ZipArchiveEntry entry) : base(path, nodeType)
        {
            mArchiveEntry = entry;
        }

        public ZipArchiveEntry mArchiveEntry;
    }

    public class ZipNodeTree : INodeTree
    {
        public Dictionary<int, Node> FlatTree { get { return mNodeTree; } }
        public bool IsReady { get; internal set; }

        public bool ReadOnly { get { return true; } }

        private ZipArchive mArchive;
        private Dictionary<int, Node> mNodeTree;


        public ZipNodeTree()
        {
            mNodeTree = new Dictionary<int, Node>();
            IsReady = false;
        }

        public bool Build(string root)
        {
            IsReady = false;
            mNodeTree.Clear();

            if (File.Exists(root))
            {
                mArchive = ZipArchive.Open(root);

                foreach (var entry in mArchive.Entries)
                {
                    string normalizedName = entry.Key.Replace('/', '\\');

                    if (entry.IsDirectory)
                    {
                        normalizedName = normalizedName.TrimEnd('\\');
                        mNodeTree.Add(normalizedName.GetHashCode(), new ZipNode(normalizedName, Node.INodeType.Directory));
                    }
                    else
                    {
                        mNodeTree.Add(normalizedName.GetHashCode(), new ZipNode(normalizedName, Node.INodeType.File, entry));
                    }
                }
                IsReady = true;
            }

            return IsReady;
        }

        public bool DirExists(string path)
        {
            bool result = mNodeTree.TryGetValue(path.GetHashCode(), out Node node);

            return result && node.NodeType == Node.INodeType.Directory;
        }

        public bool FileExists(string path)
        {
            bool result = mNodeTree.TryGetValue(path.GetHashCode(), out Node node);

            return result && node.NodeType == Node.INodeType.File;
        }

        public Stream Open(string path)
        {
            Stream result = null;
            if (mNodeTree.TryGetValue(path.GetHashCode(), out Node node))
            {
                ZipNode zipNode = node as ZipNode;
                result = zipNode.mArchiveEntry.OpenEntryStream();
            }
            return result;
        }

        /// <summary>
        /// Zip files - We're treating this as a read-only store
        /// </summary>
        public bool Write(string filename, byte[] buffer, int length)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Zip files - We're treating this as a read-only store
        /// </summary>
        public bool Delete(string filename)
        {
            throw new System.NotImplementedException();
        }
    }
}
