using System;
using System.Collections.Generic;
using System.Threading;

namespace HashFiles
{
    public class MyConcurrentQueue<T>
    {
        private Queue<T> queue = new Queue<T>();
        private AutoResetEvent ready = new AutoResetEvent(false);
        public AutoResetEvent Ready { get => ready; }

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


