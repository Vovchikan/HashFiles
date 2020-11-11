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
            for (int i = 0; i < threads.Count(); i++)
            {
                Thread compute = new Thread(new ThreadStart(() =>
                {
                    try
                    {
                        while (stash.Count > 0 || stash.IsProducering)
                        {
                            if (verbose) PrintStageOfWork("compute", null);
                            ComputeHashSum();
                        }
                    }
                    catch (Exception e) { HandleException(e); }
                    finally 
                    { 
                        results.KickProducer();
                        if (verbose)
                            PrintStageOfWork("workIsDone",
                                Thread.CurrentThread.ManagedThreadId.ToString());
                    }
                }));
                threads[i]=compute;
                compute.Start();
            }
        }

        private void ComputeHashSum()
        {
            string fullFilePath;
            bool successed = stash.TryDequeue(out fullFilePath);
            if (successed)
            {
                if (verbose) PrintStageOfWork("dequeue", fullFilePath);
                var result = HashFunction.ComputeMD5Checksum(fullFilePath);
                if (verbose) PrintStageOfWork("hashResult", result.ToStringShort());
                results.Enqueue(result);
                results.Ready.Set();
            }
            else if (stash.IsProducering)
                stash.Ready.WaitOne();
            else
                return;
        }

        private void PrintStageOfWork(string stage, string att)
        {
            string owner = $"Thrd<{Thread.CurrentThread.ManagedThreadId}>";
            switch (stage)
            {
                case "join":
                    Console.WriteLine($"{owner}: WAITING thrd<{att}>.");
                    break;
                case "dequeue":
                    FileInfo fi = new FileInfo(att);
                    Console.WriteLine($"{owner}: ENQUQUED {{{fi.Name}}}.");
                    break;
                case "hashResult":
                    Console.WriteLine($"{owner}: SENDING {{{att}}} to writer.");
                    break;
                case "workIsDone":
                    Console.WriteLine($"{owner}: FINISHED his work.");
                    break;
                case "compute":
                    Console.WriteLine($"{owner}: COMPUTING hash.");
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private void HandleException(Exception e)
        {
            throw new NotImplementedException();
            // todo Implement handle exception in CALCULATOR
        }
    }
}
