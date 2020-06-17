using HashFiles.src;
using System.Threading;

namespace HashFiles
{
    public class ThreadFileCollector
    {
        public string[] paths;
        private static Thread thread;
        private FileCollector collector;

        public ThreadFileCollector(params string[] paths)
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
                collector = new RecursiveFileCollector();
                collector.SetStash(stash);
                collector.CollectFrom(paths);
            });
            thread.Start();
        }
    }
}
