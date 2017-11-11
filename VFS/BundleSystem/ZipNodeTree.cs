using System.Collections.Generic;

using SharpCompress.Archives.Zip;

using VFS.Core;
using System.IO;

namespace VFS.BundleSystem
{
    public class ZipNode : INode
    {
        public ZipNode(string path, Type nodeType) : base(path, nodeType)
        {
            mArchiveEntry = null;
        }

        public ZipNode(string path, Type nodeType, ZipArchiveEntry entry) : base(path, nodeType)
        {
            mArchiveEntry = entry;
        }

        public ZipArchiveEntry mArchiveEntry;
    }

    public class ZipNodeTree : INodeTree
    {
        public Dictionary<int, INode> FlatTree { get { return mNodeTree; } }
        public bool IsReady { get; internal set; }

        private ZipArchive mArchive;
        private Dictionary<int, INode> mNodeTree;


        public ZipNodeTree()
        {
            mNodeTree = new Dictionary<int, INode>();
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
                        mNodeTree.Add(normalizedName.GetHashCode(), new ZipNode(normalizedName, INode.Type.Directory));
                    }
                    else
                    {
                        mNodeTree.Add(normalizedName.GetHashCode(), new ZipNode(normalizedName, INode.Type.File, entry));
                    }
                }
                IsReady = true;
            }

            return IsReady;
        }

        public bool DirExists(string path)
        {
            INode node;
            bool result = mNodeTree.TryGetValue(path.GetHashCode(), out node);

            return result && node.NodeType == INode.Type.Directory;
        }

        public bool FileExists(string path)
        {
            INode node;
            bool result = mNodeTree.TryGetValue(path.GetHashCode(), out node);

            return result && node.NodeType == INode.Type.File;
        }

        public Stream Open(string path)
        {
            Stream result = null;
            INode node;
            if (mNodeTree.TryGetValue(path.GetHashCode(), out node))
            {
                ZipNode zipNode = node as ZipNode;
                result = zipNode.mArchiveEntry.OpenEntryStream();
            }
            return result;
        }

    }
}
