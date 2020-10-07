using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Data.SqlClient;
using System.Linq;

namespace HashFiles
{
    class Program
    {
        private static MyTaskQueue<string> hashSums = new MyTaskQueue<string>();
        private delegate string MyHashFunc(string fileName);
        private static string sep = "_";

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Запуск без парамметров.");
                Console.Write("Введите катологи\\файлы (через пробел): ");
                args = Console.ReadLine().Split(' ');
            }

            var f = runThreadForFindFiles(args);

            // Рассчитать хэш-суммы для файлов
            var thrs = runThreadsForHF(f);
            // Добавить файлы в бд
            var bdWriter = runThreadForWriteToBD(thrs);

            f.Join();
            foreach (var thr in thrs)
                thr.Join();
            bdWriter.Join();

            Console.WriteLine("Работа закончена.");
            Console.ReadKey();

        }

        private static Thread runThreadForFindFiles(params string[] args)
        {
            var f = new Thread(new ThreadStart(() =>
            {
                try
                {
                    FindFiles.GetFiles(args);
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }));
            f.Start();
            return f;
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
                errors = string.IsNullOrEmpty(errors) ? "NoErrors." : errors;
                hashSum = string.IsNullOrEmpty(errors) ? "NoHashsum" : hashSum; // проверить!
                res = string.Join(sep, fileName, hashSum, errors);
            }
            return res;
        }

        private static void addHashSum(string res)
        {
            hashSums.Enqueue(res);
        }

        private static Thread[] runThreadsForHF(Thread threadFindFiles, int computeThreadsCount=2)
        {
            Thread[] th = new Thread[computeThreadsCount];
            for (int i = 0; i < computeThreadsCount; i++)
            {
                Thread compute = new Thread(new ThreadStart(() =>
                {
                    while (threadFindFiles.ThreadState == ThreadState.Running || 
                    FindFiles.files.Count() > 0)
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
                }));
                compute.Start();
                th[i] = compute;
            }
            return th;
        }
        #endregion

        private static Thread runThreadForWriteToBD(Thread[] computeThreads)
        {
            Thread bdWriter = new Thread(new ThreadStart(() =>
            {
                var helper = new MySqlServerHelper();
                using (SqlConnection sqlCon = helper.NewConnection())
                {
                    sqlCon.Open();
                    helper.TryCreateTable(sqlCon);
                    while (computeThreads.Any(w => w.ThreadState == ThreadState.Running) || hashSums.Count() > 0)
                    {
                        try
                        {
                            string[] res = hashSums.DequeueAll();
                            foreach (string stringParametr in res)
                            {
                                var parametrs = stringParametr.Split(sep.ToCharArray(),
                                    StringSplitOptions.RemoveEmptyEntries);
                                if(!helper.CheckContains(sqlCon, parametrs))
                                    helper.AddHashSum(sqlCon, parametrs);
                            }
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine("Error with message - {0}\n{1}", ex.Message, ex.StackTrace);
                        }
                    }
                }
            }));
            bdWriter.Start();
            return bdWriter;
        }

    }
}
