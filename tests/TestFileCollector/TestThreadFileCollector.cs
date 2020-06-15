using HashFiles;
using NUnit.Framework;

namespace TestCollectingFiles
{
    [TestFixture]
    public class TestThreadFileCollector
    {
        private static MyConcurrentQueue<string> stash;
        private static ThreadFileCollector dirCollector;

        [SetUp]
        public void InitializationStaticFields()
        {
            GlobalVars.InitTempDir();
            stash = new MyConcurrentQueue<string>();
            dirCollector = new ThreadFileCollector(GlobalVars.tempDirPath);
        }

        [Test]
        public void CountTempFilesWhenTempDir()
        {
            dirCollector.CollectFilesToStash(stash);
            dirCollector.Join();
            Assert.AreEqual(GlobalVars.tempFilesCount, stash.Count);
        }
    }
}
