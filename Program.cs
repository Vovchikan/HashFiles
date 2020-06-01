using System;
using System.Threading;
using System.Data.SqlClient;
using System.Linq;

namespace HashFiles
{
    class Program
    {
        private static MyConcurrentQueue<string> fullFilePaths = new MyConcurrentQueue<string>();
        private static MyConcurrentQueue<HashFunctionResult> hashSums = new MyConcurrentQueue<HashFunctionResult>();
        private delegate string MyHashFunc(string file);

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Запуск без парамметров.");
                Console.Write("Введите катологи\\файлы (через пробел): ");
                args = Console.ReadLine().Split(' ');
            }

            Thread threadFilesCollection = runThreadToCollectFiles(args);

            Thread[] threadsHashSumsCalculation = runThreadsToCalculateHashSums(threadFilesCollection);
            
            Thread bdWriter = runThreadForWriteToBD(threadsHashSumsCalculation);

            threadFilesCollection.Join();
            foreach (var thr in threadsHashSumsCalculation)
                thr.Join();
            bdWriter.Join();

            Console.WriteLine("Работа закончена.");
            Console.ReadKey();

        }

        private static Thread runThreadToCollectFiles(params string[] paths)
        {
            var f = new Thread(new ThreadStart(() =>
            {
                try
                {
                    fullFilePaths = RecursiveFilesCollector.GetFileQueue(paths);
                }
                finally
                {
                    Thread.Sleep(1000);
                }
            }));
            f.Start();
            return f;
        }

        private static Thread[] runThreadsToCalculateHashSums(Thread threadFilesCollector, int computeThreadsCount=2)
        {
            Thread[] th = new Thread[computeThreadsCount];
            for (int i = 0; i < computeThreadsCount; i++)
            {
                Thread compute = new Thread(new ThreadStart(() =>
                {
                    while (threadFilesCollector.ThreadState == ThreadState.Running || 
                    fullFilePaths.Count() > 0)
                    {
                        try
                        {
                            var fullFilePath = fullFilePaths.Dequeue();
                            var result = HashFunction.ComputeMD5Checksum(fullFilePath);
                            hashSums.Enqueue(result);
                        }
                        catch (InvalidOperationException ex) when (ex.Message == "Empty")
                        {
                            //Если хранилище пустое - ждать
                            Thread.Sleep(100);
                        }
                    }
                }));
                compute.Start();
                th[i] = compute;
            }
            return th;
        }

        private static Thread runThreadForWriteToBD(Thread[] computeThreads)
        {
            Thread bdWriter = new Thread(new ThreadStart(() =>
            {
                var helper = new MySqlServerHelper(GetConnectionStringForLocalDB());
                using (SqlConnection sqlCon = helper.NewConnection())
                {
                    sqlCon.Open();
                    helper.TryCreateTable(sqlCon);
                    while (computeThreads.Any(w => w.ThreadState == ThreadState.Running) || hashSums.Count() > 0)
                    {
                        try
                        {
                            HashFunctionResult[] results = hashSums.DequeueAll();
                            foreach (var result in results)
                            {
                                if(!helper.CheckContains(sqlCon, result))
                                    helper.AddHashSum(sqlCon, result);
                            }
                            if (results.Length == 0) Thread.Sleep(2000);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("ERROR MESSAGE - {0}\n{1}", ex.Message, ex.StackTrace);
                        }
                    }
                }
            }));
            bdWriter.Start();
            return bdWriter;
        }

        private static String GetConnectionStringForLocalDB()
        {
            return @"Data Source = (localdb)\MSSQLLocalDB;
                AttachDbFilename=D:\Programming\C#\Github-portfolio\HashFiles\Database1.mdf;
                Integrated Security=True;Connect Timeout=30;";
        }
    }
}
