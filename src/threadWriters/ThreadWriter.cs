using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace HashFiles.src.threadWriters
{
    public class ThreadWriter
    {
        private MyConcurrentQueue<HashFunctionResult> stash;
        private ThreadHashSumCalculator calc;
        private ConnectionWith connection;
        private Thread thread;
        private readonly int waitingTime = 200;

        public void Join()
        {
            thread.Join();
        }
        public void StartFromTo(ThreadHashSumCalculator calc,
            MyConcurrentQueue<HashFunctionResult> stash, ConnectionWith wrCon)
        {
            this.stash = stash;
            this.calc = calc;
            this.connection = wrCon;
            thread = new Thread(new ThreadStart(() =>
            {
                TryAddingDataFromStash();
            }));
            thread.Start();
        }

        private bool IsAllowed()
        {
            return calc.GetThreads().Any(w => w.ThreadState == ThreadState.Running)
                || stash.Count > 0;
        }

        private void TryAddingDataFromStash()
        {
            try
            {
                while (IsAllowed())
                {
                    AddingDataFromStash();
                }
            }
            catch(EmptyConcurrentQueueException)
            {
                // todo (threadWriter) реализовать ожидание через EventWaitHandle
                Thread.Sleep(waitingTime);
                TryAddingDataFromStash();
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
