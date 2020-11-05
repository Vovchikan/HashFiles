using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HashFiles
{
    public class ThreadHashSumCalculator
    {
        private static Thread[] threads;
        private MyConcurrentQueue<string> stash;
        private MyConcurrentQueue<HashFunctionResult> results;
        private ThreadFileCollector collector;
        private readonly int waitTime=200;

        public ThreadHashSumCalculator(int count)
        {
            threads = new Thread[count];
        }

        public void Join()
        {
            foreach(var thread in threads)
                thread.Join();
        }

        public Thread[] GetThreads()
        {
            return threads;
        }

        public void StartComputingFromTo(ThreadFileCollector collector, MyConcurrentQueue<string> stash, 
            MyConcurrentQueue<HashFunctionResult> results)
        {
            this.stash = stash;
            this.results = results;
            this.collector = collector;
            for (int i = 0; i < threads.Count(); i++)
            {
                Thread compute = new Thread(new ThreadStart(() =>
                {
                    while (IsAllowed())
                    {
                        TryComputeHashSum();
                    }
                }));
                threads[i]=compute;
                compute.Start();
            }
        }

        private bool IsAllowed()
        {
            return collector.ThreadState == ThreadState.Running ||
                    stash.Count > 0;
        }

        private void TryComputeHashSum()
        {
            try
            {
                var fullFilePath = stash.Dequeue();
                var result = HashFunction.ComputeMD5Checksum(fullFilePath);
                results.Enqueue(result);
            }
            catch (EmptyConcurrentQueueException)
            {
                // todo (threadWriter) реализовать ожидание через EventWaitHandle
                Thread.Sleep(waitTime);
            }
        }
    }
}
