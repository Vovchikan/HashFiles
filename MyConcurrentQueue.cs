using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashFiles
{
    public class MyConcurrentQueue<T>
    {
        private Queue<T> queue = new Queue<T>();

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
                if (queue.Count == 0) throw new InvalidOperationException("Empty");
                return queue.Dequeue();
            }
        }

        public virtual Tuple<T, int> getTupleOfDequeueAndCount()
        {
            lock (this)
            {
                return new Tuple<T, int>(this.Dequeue(), this.Count());
            }
        }

        public virtual int Count()
        {
            lock (this)
            {
                return queue.Count;
            }
        }

        public virtual T[] DequeueAll()
        {
            lock (this)
            {
                var res = queue.ToArray();
                queue.Clear();
                return res;
            }
        }
    }
}


