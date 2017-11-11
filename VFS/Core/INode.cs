namespace VFS.Core
{
    public class INode
    {
        // Bitfield for node types
        public const byte kReadOnly = 1 << 0;
        public const byte kBinary = 1 << 1;

        public enum Type
        {
            Directory,
            File,
            Undef
        };

        public INode(string path, Type nodeType)
        {
            mFilename = path;
            mNodetype = nodeType;
    }

        public string Name { get { return mFilename; } }
        public Type NodeType { get { return mNodetype; } }

        private Type mNodetype;
        private string mFilename;
    }
}
