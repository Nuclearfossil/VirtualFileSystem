using System.IO;

namespace VFS.Core
{
    public interface INodeTree
    {
        bool DirExists(string path);
        bool FileExists(string path);
        Stream Open(string filename);
    }
}
