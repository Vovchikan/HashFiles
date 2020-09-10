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
            dirCollector = new ThreadFileCollector(true);
        }

        [Test]
        public void CountTempFilesWhenTempDir()
        {
            dirCollector.ExecuteToFrom(stash, GlobalVars.tempDirPath);
            dirCollector.Join();
            Assert.AreEqual(GlobalVars.tempFilesCount, stash.Count);
        }
    }
}
