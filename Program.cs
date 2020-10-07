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

                // Рассчитать хэш-суммы для файлов
                Task compute = Task.Run(() =>
                {
                    int qc = 0;
                    while (search || qc > 0)
                    {
                        try
                        {
                            var res = FindFiles.TakeFromStorage();
                            qc = res.Item2;
                            computeHashSums(HashFunc.ComputeMD5Checksum, res.Item1);
                        }
                        catch (InvalidOperationException)
                        {
                            //Если хранилище пустое - ждать
                            Task.Delay(300);
                        }
                    }
                });

                // Добавить файлы в бд

                Task.WaitAll(find, compute);
            }

            Console.WriteLine("Работа закончена.");
            Console.ReadKey();

        }

        private static void computeHashSums(MyHashFunc hf, string fileName)
        {
            string  hashSum ="", errors="";
            try
            {
                hashSum = hf(fileName);
            }
            catch (HashFunc.HashFuncException ex)
            {
                // Поймать исключение у хэш-функции
                errors = ex.Message;
            }
            finally
            {
                errors = string.IsNullOrEmpty(errors) ? "No errors." : errors;
                hashSum = string.IsNullOrEmpty(errors) ? "No hashsum" : hashSum;
                addHashSum(hashSum, fileName, errors);
            }
        }

        private static void addHashSum(string hashSum, string fileName, string errors="", string sep=" ")
        {
            lock (hashSums)
            {
                var res = string.Join(sep, fileName, hashSum, errors);
                hashSums.Enqueue(res);
                Console.WriteLine(res);
            }
        }

    }
}
