using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashFiles
{
    class Program
    {
        private static Queue<string> hashSums = new Queue<string>();
        private delegate string MyHashFunc(string fileName);
        private static bool search = true;

        static void Main(string[] args)
        {
            if(args.Length == 0)
            {
                Console.WriteLine("Запуск без парамметров.");
            }
            else
            {
                // Начать поиска файлов
                Task find = Task.Run(() =>
                {
                    try
                    {
                        FindFiles.GetFiles(args);
                    }
                    finally
                    {
                        search = false;
                    }
                });
                Task.Run(() =>
                {
                    try
                    {
                        ComputeHashSums(HashFunc.ComputeMD5Checksum2, FindFiles.files);
                    }
                    finally
                    {
                        ;
                    }
                });
            }

            Console.ReadKey();
            lock (hashSums)
            {
                while(hashSums.Count > 0)
                {
                    Console.WriteLine(hashSums.Dequeue());
                }
            }
            Console.ReadKey();
        }

        private static void ComputeHashSums(MyHashFunc hf, Queue<string> queue)
        {
            string fileName;
            int qc = 0;
            while (search || qc > 0) {
                try
                {
                    lock (queue)
                    {
                        fileName = queue.Dequeue();
                        qc = queue.Count;
                    }
                    string res = hf(fileName);
                    AddHashSum(res, fileName);
                }
                catch (InvalidOperationException)
                {
                    Task.Delay(300);
                }
            }
            
        }

        private static void AddHashSum(string hashSum, string fileName)
        {
            lock (hashSums)
            {
                hashSums.Enqueue(string.Join(" ", fileName, hashSum));
            }
        }
    }
}
