using System;
using System.Threading;
using System.Linq;
using System.IO;

namespace HashFiles
{
    class Program
    {
        private static MyConcurrentQueue<string> filePathsStash;
        private static MyConcurrentQueue<HashFunctionResult> hashSums = new MyConcurrentQueue<HashFunctionResult>();

        static void Main(string[] args)
        {
            if(args.Length == 0)
                args = InitArgs();
            
            filePathsStash = new MyConcurrentQueue<string>();
            ThreadFileCollector threadCollector = new ThreadFileCollector(args);
            threadCollector.CollectFilesToStash(filePathsStash);

            var calculator = new ThreadHashSumCalculator(2);
            calculator.StartComputingFromTo(threadCollector, filePathsStash, hashSums);
            
            Thread bdWriter = runThreadForWriteToBD(calculator.GetThreads());

            threadCollector.Join();
            calculator.Join();
            bdWriter.Join();

            Console.WriteLine("Работа закончена.");
            Console.ReadKey();

        }

        private static string[] InitArgs()
        {
            Console.WriteLine("Запуск без парамметров.");
            Console.Write("Введите катологи\\файлы (через пробел): ");
            string[] args = Console.ReadLine().Split(' ');
            return args;
        }

        private static Thread runThreadForWriteToBD(Thread[] computeThreads)
        {
            Thread bdWriter = new Thread(new ThreadStart(() =>
            {
                var helper = new MySqlServerHelper(GetConnectionStringForLocalDB());
                helper.NewConnection();
                helper.OpenConnection();
                helper.TryCreateTable();
                while (computeThreads.Any(w => w.ThreadState == ThreadState.Running) || hashSums.Count > 0)
                {
                    TryAddingNewResults(helper);
                }
                helper.CloseAndDisposeConnection();
            }));
            bdWriter.Start();
            return bdWriter;
        }

        private static String GetConnectionStringForLocalDB()
        {
            string connectiongString;
            using (var sw = new StreamReader("./data/connectingString.txt"))
            {
                connectiongString = sw.ReadToEnd();
                if (String.IsNullOrEmpty(connectiongString))
                    throw new FileNotFoundException("Wrong file. This file is empty!");
            }
            return connectiongString;
        }

        private static void TryAddingNewResults(MySqlServerHelper sqlHelper)
        {
            try
            {
                HashFunctionResult[] results = hashSums.DequeueAll();
                foreach (var result in results)
                {
                    sqlHelper.AddHashSum(result);
                }
                if (results.Length == 0) Thread.Sleep(2000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR MESSAGE - {0}\n{1}", ex.Message, ex.StackTrace);
            }
        }
    }
}
