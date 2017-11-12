namespace VFS.Core
{
    public class Node
    {
        // Bitfield for node types
        public const byte kReadOnly = 1 << 0;
        public const byte kBinary = 1 << 1;

        public enum INodeType
        {
            Directory,
            File,
            Undef
        };

        public Node(string path, INodeType nodeType)
        {
            mFilename = path;
            mNodetype = nodeType;
        }

        public string Name { get { return mFilename; } }
        public INodeType NodeType { get { return mNodetype; } }

        private INodeType mNodetype;
        private string mFilename;
    }
}
