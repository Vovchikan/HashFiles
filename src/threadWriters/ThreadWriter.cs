using System;
using System.Data.SqlClient;
using System.Threading;

namespace HashFiles.src.threadWriters
{
    public class ThreadWriter
    {
        private MyConcurrentQueue<HashFunctionResult> stash;
        private ConnectionWith connection;
        private Thread thread;
        private bool verbose;

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
                try
                {
                    while (stash.Count > 0 || stash.IsProducering)
                    {
                        AddingDataFromStash();
                    }
                }
                catch (Exception e) { HandleException(e); }
                finally
                {
                    if (verbose)
                        Console.WriteLine($"Thrd<{Thread.CurrentThread.ManagedThreadId}>-Writer: FINISHED his work.");
                }
            }));
            thread.Start();
        }

        private void AddingDataFromStash()
        {
            if (stash.Count > 0)
            {
                HashFunctionResult[] results = stash.DequeueAll();
                foreach (var result in results)
                {
                    connection.SendHashData(result);
                }
            }
            else
                stash.Ready.WaitOne();
        }

        private void HandleException(Exception e)
        {
            if (e is SqlException)
                Console.WriteLine($"ERROR MESSAGE: {e.Message}");
            else
            {
                Console.WriteLine($"ERROR MESSAGE: {e.Message}\n" +
                    $"STACKTRACE: {e.StackTrace}");
                throw e;
            }
        }
    }
}
