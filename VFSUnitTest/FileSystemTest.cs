using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using VFS.BundleSystem;
using VFS.Core;
using VFS.FileSystem;

namespace VFSUnitTest
{
    [TestClass]
    public class FileSystemTest
    {
        [TestMethod]
        public void TestDirectoryHierarchy()
        {
            var fileSystem = new FileSystem();

            var nodeOne = new NodeTree();
            var nodeTwo = new NodeTree();
            var nodeThree = new ZipNodeTree();

            Assert.IsTrue(nodeOne.Build(@"E:\dev\csharp\vfs\VirtualFileSystem\VFSUnitTest\testdata\Data\"));
            Assert.IsTrue(nodeTwo.Build(@"E:\dev\csharp\vfs\VirtualFileSystem\VFSUnitTest\testdata\Data2\"));
            Assert.IsTrue(nodeThree.Build(@"E:\dev\csharp\vfs\VirtualFileSystem\VFSUnitTest\testdata\Data.zip"));

            fileSystem.AddSystem("First", nodeOne);
            fileSystem.AddSystem("Second", nodeTwo);
            fileSystem.AddSystem("Third", nodeThree);

            Assert.IsTrue(fileSystem.DirExists(@"subfolder01"));
            Assert.IsTrue(fileSystem.DirExists(@"subfolder02"));
            Assert.IsTrue(fileSystem.DirExists(@"alphabetagamma"));
            Assert.IsTrue(fileSystem.DirExists(@"subfolder02\subfolder_a"));
            Assert.IsTrue(fileSystem.DirExists(@"subfolder02\subfolder_a\subfolder_a_b"));
            Assert.IsTrue(fileSystem.DirExists(@"subfolder02\subfolder_a\subfolder_a_c"));

            Assert.IsTrue(fileSystem.FileExists(@"colours.txt"));
            Assert.IsTrue(fileSystem.FileExists(@"File01.txt"));
            Assert.IsTrue(fileSystem.FileExists(@"subfolder02\booga.txt"));
            Assert.IsTrue(fileSystem.FileExists(@"subfolder02\subfolder_a\geoip.txt"));
            Assert.IsTrue(fileSystem.FileExists(@"subfolder02\subfolder_a\subfolder_a_b\another_test.folder.txt"));
            Assert.IsTrue(fileSystem.FileExists(@"subfolder01\map01.txt"));

            // Get Data from various filesystems
            // This should be coming from nodeOne
            using (Stream fs = fileSystem.Open(@"alphabetagamma\db.sample.txt"))
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

            // This should be coming from nodeTwo
            using (Stream fs = fileSystem.Open(@"File01.txt"))
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

            // This should be coming from nodeThree
            using (Stream fs = fileSystem.Open(@"subfolder02\subfolder_a\subfolder_a_c\data.data.data.txt"))
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
        }
    }
}
