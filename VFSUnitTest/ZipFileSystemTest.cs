using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using VFS.BundleSystem;
using VFS.Core;

namespace VFSUnitTest
{
    [TestClass]
    public class ZipFileSystemTest
    {
        [TestMethod]
        public void TestZipFile()
        {
            ZipNodeTree tree = new ZipNodeTree();

            string appPath = Path.Combine(TestingUtils.GetTestingBaseFolder(), "testdata");

            Assert.IsTrue(tree.Build(Path.Combine(appPath, @"Data.zip")));

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
            Assert.IsTrue(tree.DirExists(@"subfolder02\subfolder_a\subfolder_a_c"));

            Assert.IsFalse(tree.DirExists(@"AttributesCrap"));

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
        }
    }
}
