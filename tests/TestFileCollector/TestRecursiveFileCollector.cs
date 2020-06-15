using HashFiles;
using NUnit.Framework;
using System;
using System.IO;

namespace TestCollectingFiles
{
    [TestFixture]
    public class TestRecursiveFileCollector
    {
        private static RecursiveFileCollector recursiveCollector;
        private static MyConcurrentQueue<string> stash;

        [OneTimeSetUp]
        public void InitizalizationFields()
        {
            GlobalVars.InitTempDir();
            recursiveCollector = new RecursiveFileCollector();
        }

        [SetUp]
        public void UpdateStaticFields()
        {
            stash = new MyConcurrentQueue<string>();
            recursiveCollector.SetStash(stash);
        }

        [Test]
        public void TempFilesCount()
        {
            recursiveCollector.CollectFrom(GlobalVars.tempDirPath);
            Assert.AreEqual(GlobalVars.tempFilesCount, stash.Count);
        }

        [Test]
        public void GetExceptionWhenArgIsEmptyString()
        {
            Assert.Throws<ArgumentException>(() => recursiveCollector.CollectFrom(""));
        }

        [Test]
        public void GetExceptionWhenFakeFileInput()
        {
            Assert.Throws<ArgumentException>(() => recursiveCollector.CollectFrom("fake-file.txt"));
        }
    }
}
