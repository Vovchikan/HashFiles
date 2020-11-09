using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace HashFiles.src.threadWriters
{
    public class ThreadWriter
    {
        private MyConcurrentQueue<HashFunctionResult> stash;
        private ConnectionWith connection;
        private Thread thread;
        private bool verbose;
        private bool hasWork = true;

        public ThreadWriter() { }

        public ThreadWriter(bool verbose)
        {
            this.verbose = verbose;
        }

        public void Join()
        {
            thread.Join();
        }
        public void StartFromTo(MyConcurrentQueue<HashFunctionResult> stash, 
            ConnectionWith wrCon)
        {
            this.stash = stash;
            this.connection = wrCon;
            thread = new Thread(new ThreadStart(() =>
            {
                while (hasWork)
                {
                    TryAddingDataFromStash();
                }
                if (verbose)
                    Console.WriteLine($"Writer-Thrd<{Thread.CurrentThread.ManagedThreadId}> FINISHED his work.");
            }));
            thread.Start();
        }

        private void TryAddingDataFromStash()
        {
            try
            {
                AddingDataFromStash();
            }
            catch (EmptyConcurrentQueueException)
            {
                Timer tmr = new Timer(id =>
                {
                    hasWork = false;
                    stash.Ready.Set();
                }, Thread.CurrentThread.ManagedThreadId.ToString(), 5000, Timeout.Infinite);
                stash.Ready.WaitOne();
                tmr.Dispose();
            }
            catch (SqlException e)
            {
                Console.WriteLine($"ERROR MESSAGE: {e.Message}");
                // todo writer не должен перехватывать sqlexception, нужно придумать своё общее исключение
            }
            catch (Exception e)
            {
                Console.WriteLine($"ERROR MESSAGE: {e.Message}\n" +
                    $"STACKTRACE: {e.StackTrace}");
                throw e;
            }
        }

        private void AddingDataFromStash()
        {
            HashFunctionResult[] results = stash.DequeueAll();
            foreach (var result in results)
            {
                connection.SendHashData(result);
            }
        }
    }
}
