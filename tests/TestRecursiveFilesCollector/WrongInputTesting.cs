using NUnit.Framework;
using HashFiles;
using System.IO;
using System;

namespace TestRecursiveFilesCollector
{
    [TestFixture]
    public class WrongInputTesting
    {

        [OneTimeSetUp]
        public void CreateTempDirsAndFiles()
        {
        }

        [Test]
        public void GetExceptionWhenArgIsEmptyString()
        {
            Assert.Throws<ArgumentException>(() => RecursiveFilesCollector.GetFileQueue(""));
        }

        [OneTimeTearDown]
        public void DeleteTempDirsAndFile()
        {
        }
    }
}
