using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using VFS.FileSystem;

namespace VFSUnitTest
{
    [TestClass]
    public class DiskFileSystemTest
    {
        [TestMethod]
        public void TestFileSystem()
        {
            string appPath = Path.Combine(TestingUtils.GetTestingBaseFolder(), "testdata");

            var rootPath = Path.Combine(appPath, @"Data\");

            // Cleanup from old run (just in case - we want a clean run)
            var tempDir = Path.Combine(rootPath, "temp");
            if (Directory.Exists(tempDir))
            {
                Directory.Delete(tempDir, true);
            }

            NodeTree tree = new NodeTree();
            Assert.IsTrue(tree.Build(rootPath));

            foreach (var item in tree.FlatTree)
            {
                Console.WriteLine("{0}: {1}", item.Value.NodeType.ToString(), item.Value.Name);
            }

            Console.WriteLine("End list of contents");

            Assert.IsTrue(tree.FileExists(@"colours.txt"));
            Assert.IsTrue(tree.DirExists(@"subfolder01"));
            Assert.IsTrue(tree.DirExists(@"subfolder02"));
            Assert.IsTrue(tree.DirExists(@"alphabetagamma"));
            Assert.IsTrue(tree.DirExists(@"subfolder02\subfolder_a"));
            Assert.IsTrue(tree.DirExists(@"subfolder02\subfolder_a\subfolder_a_b"));
            Assert.IsFalse(tree.DirExists(@"subfolder02\subfolder_a\subfolder_a_c"));

            using (Stream fs = tree.Open(@"alphabetagamma\db.sample.txt"))
            {
                byte[] buffer = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                int amountRead = fs.Read(buffer, 0, buffer.Length);
                int accumulation = amountRead;
                while (amountRead > 0)
                {
                    string contents = temp.GetString(buffer, 0, amountRead);
                    Console.Write(contents);
                    amountRead = fs.Read(buffer, 0, buffer.Length);
                    accumulation += amountRead;
                }
                Console.WriteLine("");
                Console.WriteLine("Read {0} bytes", accumulation);
            }

            Console.WriteLine("Next Test run");
            using (Stream fs = tree.Open(@"colours01.txt"))
            {
                byte[] buffer = new byte[1024];
                UTF8Encoding temp = new UTF8Encoding(true);

                int amountRead = fs.Read(buffer, 0, buffer.Length);
                int accumulation = amountRead;
                while (amountRead > 0)
                {
                    Console.WriteLine(temp.GetString(buffer, 0, amountRead));
                    amountRead = fs.Read(buffer, 0, buffer.Length);
                    accumulation += amountRead;
                }
                Console.WriteLine("");
                Console.WriteLine("Read {0} bytes", accumulation);
            }

            Byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
            Assert.IsTrue(tree.Write(@"temp\sample_file.txt", info, info.Length));
            Assert.IsTrue(tree.FileExists(@"temp\sample_file.txt"));
            Assert.IsTrue(tree.Delete(@"temp\sample_file.txt"));
            Assert.IsFalse(tree.FileExists(@"temp\sample_file.txt"));
            Assert.IsFalse(File.Exists(Path.Combine(rootPath, @"temp\sample_file.txt")));
        }
    }
}
