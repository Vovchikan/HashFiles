using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace HashFiles
{
    class Program
    {
        private static MyTaskQueue<string> hashSums = new MyTaskQueue<string>();
        private delegate string MyHashFunc(string fileName);

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
                        Task.Delay(1000);
                    }
                });

                // Рассчитать хэш-суммы для файлов
                Action acompute = () =>
                {
                    while (find.Status == TaskStatus.Running || FindFiles.files.Count() > 0)
                    {
                        try
                        {
                            var fileName = FindFiles.files.Dequeue();
                            computeHashSums(HashFunc.ComputeMD5Checksum, fileName);
                        }
                        catch (InvalidOperationException ex) when (ex.Message == "Empty")
                        {
                            //Если хранилище пустое - ждать
                            Task.Delay(100);
                        }
                    }
                };

                int computTasksCount = 2;
                List<Task> tasks = new List<Task>();
                for (int i = 0; i < computTasksCount; i++)
                {
                    tasks.Add(Task.Run(acompute));
                }

                tasks.Add(find);

                // Добавить файлы в бд
                Task bdWriter = Task.Run(() =>
                {
                    // Бд
                });
                tasks.Add(bdWriter);

                Task.WaitAll(tasks.ToArray());
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
                hashSum = string.IsNullOrEmpty(errors) ? "No hashsum" : hashSum; // Не работает, проверить!
                addHashSum(hashSum, fileName, errors);
            }
        }

        private static void addHashSum(string hashSum, string fileName, string errors="", string sep=" ")
        {
            var res = string.Join(sep, fileName, hashSum, errors, Thread.CurrentThread.ManagedThreadId);
            hashSums.Enqueue(res);
        }

    }
}
