using System.Threading;

namespace HashFiles
{
    public class ThreadDirCollector
    {
        private static Thread thread;
        public string[] paths;

        public ThreadDirCollector(params string[] paths)
        {
            this.paths = paths;
        }

        public void Join()
        {
            thread.Join();
        }

        public Thread GetThread()
        {
            return thread;
        }

        public void CollectFilesToStash(MyConcurrentQueue<string> stash)
        {
            thread = new Thread(() =>
            {
                var recursiveCollector = new RecursiveFilesCollector(stash);
                recursiveCollector.CollectFilesToQueue(paths);
            });
            thread.Start();
        }
    }
}
