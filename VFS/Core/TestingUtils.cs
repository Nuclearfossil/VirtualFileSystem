using System;
using System.IO;

namespace VFS.Core
{
    public static class TestingUtils
    {
        private static string sAppPath = null;
        public static string GetTestingBaseFolder()
        {
            if (sAppPath == null)
            {
                sAppPath = AppDomain.CurrentDomain.BaseDirectory;
                sAppPath = Path.Combine(sAppPath, @"..\..\..\");
                sAppPath = Path.GetFullPath(sAppPath);
            }

            return sAppPath;
        }
    }
}
