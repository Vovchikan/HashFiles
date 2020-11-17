using HashFiles;
using NUnit.Framework;
using System.IO;

namespace TestCollectingFiles
{
    [TestFixture]
    public class TestThreadFileCollector
    {
        private static MyConcurrentQueue<string> stash;

        [OneTimeSetUp]
        public void InitializationStaticFields()
        {
            GlobalVars.InitTempDir();
        }

        [OneTimeTearDown]
        public void DeleteTempObjects()
        {
            Directory.Delete(GlobalVars.tempDirPath, true);
        }

        [SetUp]
        public void InitializationStash()
        {
            stash = new MyConcurrentQueue<string>();
        }

        [Test]
        public void CountTempFilesRecursive()
        {
            var collector = new ThreadFileCollector(true);
            collector.ExecuteToFrom(stash, GlobalVars.tempDirPath);
            collector.Join();
            Assert.AreEqual(GlobalVars.tempFilesCount, stash.Count);
        }

        [Test]
        public void CountTempFilesNotRecursive()
        {
            var collector = new ThreadFileCollector(false);
            collector.ExecuteToFrom(stash, GlobalVars.tempDirPath);
            collector.Join();
            Assert.AreEqual(GlobalVars.onlyParentTempFilesCount, stash.Count);
        }

        [Test]
        public void CountDirectoryFiles(
            [Values("../")]
        string directoryName)
        {
            var expected_result = Directory.GetFiles(directoryName);
            var collector = new ThreadFileCollector(false);
            collector.ExecuteToFrom(stash, directoryName);
            collector.Join();
            Assert.AreEqual(expected_result.Length, stash.Count);
        }
    }
}
