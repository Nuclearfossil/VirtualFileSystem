using System.Collections.Generic;
using System.IO;

namespace VFS.Core
{
    public class FileSystem
    {
        private Stack<INodeTree> mFileSystems;
        private const int kInitialSystemSize = 5;

        public FileSystem()
        {
            mFileSystems = new Stack<INodeTree>(kInitialSystemSize);
        }

        public bool AddSystem(string id, INodeTree nodeTree)
        {
            if (nodeTree == null)
            {
                return false;
            }

            mFileSystems.Push(nodeTree);
            return true;
        }

        public bool DirExists(string path)
        {
            bool result = false;
            if (mFileSystems.Count == 0)
            {
                return result;
            }

            // Walk the stack, looking for the file in each filesystem
            foreach (var filestytem in mFileSystems)
            {
                if (filestytem.DirExists(path))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public bool FileExists(string path)
        {
            bool result = false;
            if (mFileSystems.Count == 0)
            {
                return result;
            }

            foreach(var filesystem in mFileSystems)
            {
                if (filesystem.FileExists(path))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        public Stream Open(string filename)
        {
            Stream output = null;
            if (mFileSystems.Count == 0)
            {
                return output;
            }

            // Walk the stack, looking for the file in each filesystem
            foreach (var filestytem in mFileSystems)
            {
                output = filestytem.Open(filename);
                if (output != null)
                {
                    break;
                }
            }

            return output;
        }
    }
}
