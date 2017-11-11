using System.IO;

namespace VFS.Core
{
    public interface INodeTree
    {
        bool ReadOnly { get; }
        bool DirExists(string path);
        bool FileExists(string path);

        bool Write(string filename, byte[] buffer, int length);
        bool Delete(string filename);

        Stream Open(string filename);
    }
}
