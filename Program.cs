using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Data.SqlClient;

namespace HashFiles
{
    class Program
    {
        private static MyTaskQueue<string> hashSums = new MyTaskQueue<string>();
        private delegate string MyHashFunc(string fileName);
        private static string sep = "_";

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
                            var res = computeHashSums(HashFunc.ComputeMD5Checksum, fileName, sep);
                            addHashSum(res);
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

        #region work with HashFunc class
        /// <summary>
        /// Использвует хэш-функцию hf для подсчёта хэш-суммы
        /// файла fileName. Добавляет инфу об ошибках в результат.
        /// Возвращает строку - string.Join(sep, fileName, hashSum, errors).
        /// </summary>
        private static string computeHashSums(MyHashFunc hf, string fileName, string sep=" ")
        {
            string  hashSum ="", errors="", res;
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
                hashSum = string.IsNullOrEmpty(errors) ? "No hashsum" : hashSum; // проверить!
                res = string.Join(sep, fileName, hashSum, errors);
            }
            return res;
        }

        private static void addHashSum(string res)
        {
            hashSums.Enqueue(res);
        }
        #endregion

        private static void writeToBD()
        {
            var helper = new MySqlServerHelper();
            using (SqlConnection sqlCon = helper.NewConnection())
            {
                sqlCon.Open();
                helper.TryCreateTable(sqlCon);
                while (hashSums.Count() > 0)
                {
                    var res = hashSums.Dequeue().Split(sep.ToCharArray(),
                        StringSplitOptions.RemoveEmptyEntries);
                    helper.AddHashSum(sqlCon, res);
                }
            }
        }

    }
}
