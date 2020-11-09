using System;
using System.IO;
using System.Linq;
using System.Threading;

namespace HashFiles
{
    public class ThreadHashSumCalculator
    {
        private static Thread[] threads;
        private bool verbose;
        private MyConcurrentQueue<string> stash;
        private MyConcurrentQueue<HashFunctionResult> results;

        public ThreadHashSumCalculator(int count)
        {
            threads = new Thread[count];
            verbose = false;
        }

        public ThreadHashSumCalculator(int count, bool verbose)
        {
            threads = new Thread[count];
            this.verbose = verbose;
        }

        public void Join()
        {
            foreach (var thread in threads)
            {
                thread.Join();
                if (verbose)
                    PrintStageOfWork("join", thread.ManagedThreadId.ToString());
            }
        }

        public void StartComputingFromTo(MyConcurrentQueue<string> stash, 
            MyConcurrentQueue<HashFunctionResult> results)
        {
            this.stash = stash;
            this.results = results;
            bool hasWork = true;
            for (int i = 0; i < threads.Count(); i++)
            {
                
                Thread compute = new Thread(new ThreadStart(() =>
                {
                    while (hasWork)
                    {
                        hasWork = TryComputeHashSum();
                    }
                    if (verbose) 
                        PrintStageOfWork("workIsDone", 
                            Thread.CurrentThread.ManagedThreadId.ToString());
                }));
                threads[i]=compute;
                compute.Start();
            }
        }

        private bool TryComputeHashSum()
        {
            if (verbose) PrintStageOfWork("trycompute", null);
            try
            {
                var fullFilePath = stash.Dequeue();
                if (verbose) PrintStageOfWork("dequeue", fullFilePath);
                var result = HashFunction.ComputeMD5Checksum(fullFilePath);
                if (verbose) PrintStageOfWork("hashResult", result.ToStringShort());
                results.Enqueue(result);
                results.Ready.Set();
                return true;
            }
            catch (EmptyConcurrentQueueException)
            {
                bool hasWork = true;
                int threadId = Thread.CurrentThread.ManagedThreadId;
                Timer tmr = new Timer( id =>
                {
                    hasWork = false;
                    stash.Ready.Set();
                    if (verbose) PrintStageOfWork("timer", id.ToString());
                }, threadId, 5000, threads.Length);
                stash.Ready.WaitOne();
                if (verbose) PrintStageOfWork("timerDispose", null);
                tmr.Dispose();
                return hasWork;
            }
        }

        private void PrintStageOfWork(string stage, string att)
        {
            string owner = $"Thrd<{Thread.CurrentThread.ManagedThreadId}>";
            switch (stage)
            {
                case "join":
                    Console.WriteLine($"{owner}: thrd<{att}> JOINED!");
                    break;
                case "dequeue":
                    FileInfo fi = new FileInfo(att);
                    Console.WriteLine($"{owner}: FILE {{{fi.Name}}} has been ENQUQUED.");
                    break;
                case "hashResult":
                    Console.WriteLine($"{owner}: RESULT {{{att}}} is sending to writer.");
                    break;
                case "workIsDone":
                    Console.WriteLine($"{owner}: my work is DONE here.");
                    break;
                case "timer":
                    Console.WriteLine($"Timer-{owner}: this thrd<{att}> has NO WORK to do.");
                    break;
                case "timerDispose":
                    Console.WriteLine($"{owner}: DISPOSE my timer!");
                    break;
                case "trycompute":
                    Console.WriteLine($"{owner}: Try COMPUTE hash.");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
