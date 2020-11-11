using System;
using System.Collections.Generic;
using System.Threading;

namespace HashFiles
{
    public class MyConcurrentQueue<T>
    {
        private Queue<T> queue = new Queue<T>();
        private AutoResetEvent ready = new AutoResetEvent(false);
        private bool isProducering = true;
        private int producersCount = 1;
        private int consumersCount;

        public MyConcurrentQueue()
        {
        }

        public MyConcurrentQueue(int producersCount, int consumersCount)
        {
            this.producersCount = producersCount;
            this.consumersCount = consumersCount;
        }

        public AutoResetEvent Ready { get => ready; }

        public int Count
        {
            get
            {
                lock (this)
                {
                    return queue.Count;
                }
            }
        }

        public bool IsProducering 
        { 
            get 
            {
                lock (this)
                {
                    return isProducering;
                }
            }
            private set
            {
                lock (this)
                {
                    isProducering = false;
                }
                WakeUpAllConsumers();
            }
        }

        public virtual void Enqueue(T input)
        {
            lock (this)
            {
                queue.Enqueue(input);
            }
        }

        public virtual T Dequeue()
        {
            lock (this)
            {
                if (queue.Count == 0) throw new EmptyConcurrentQueueException("Collection is empty.");
                return queue.Dequeue();
            }
        }

        public bool TryDequeue(out T elem)
        {
            lock (this)
            {
                if (Count == 0)
                {
                    elem = default(T);
                    return false;
                }
                elem = queue.Dequeue();
                return true;
            }
        }


        private void WakeUpAllConsumers()
        {
            for (int i = 0; i < consumersCount; i++)
            {
                ready.Set();
            }
        }

        public void KickProducer()
        {
            lock (this)
            {
                producersCount--;
                if (producersCount == 0)
                    IsProducering = false;
            }
        }

        public virtual T[] DequeueAll()
        {
            lock (this)
            {
                if (queue.Count == 0) throw new EmptyConcurrentQueueException("Collection is empty.");
                var res = queue.ToArray();
                queue.Clear();
                return res;
            }
        }
    }

    public class EmptyConcurrentQueueException : Exception
    {
        public EmptyConcurrentQueueException(string message) : base(message) { }
    }
}


