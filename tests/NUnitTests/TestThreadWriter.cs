using System;
using System.IO;

using HashFiles.src.options;
using HashFiles.src.threadWriters;
using NUnit.Framework;

namespace NUnitTests
{
    [TestFixture]
    public class TestThreadWriter
    {
        [Test]
        public void TestFileCreation()
        {
            var options = CreateOptionsWithUniqueFileName();
            var connection = new ConnectionWithFile(options);
            connection.PrepareForWriting();
            connection.Close();
            Assert.IsTrue(File.Exists(Path.Combine(options.OutputDirPath, options.FileName)));
            DeleteUnusedDirectories(options.OutputDirPath);
        }

        private OptionsForFile CreateOptionsWithUniqueFileName()
        {
            string uniqueFileName = Path.GetRandomFileName();
            var options = new OptionsForFile(
                "./data", uniqueFileName, false,
                new string[] { "./" }, false, 2, false);
            return options;
        }

        private void DeleteUnusedDirectories(params string[] directories)
        {
            foreach (var dir in directories)
                if (Directory.Exists(dir))
                    Directory.Delete(dir, true);
        }

    }
}
