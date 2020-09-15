using System;
using HashFiles.src.threadWriters;
using System.Data.SqlClient;
using System.IO;

namespace HashFiles
{
    class Program
    {
        private static MyConcurrentQueue<string> filePathsStash;
        private static MyConcurrentQueue<HashFunctionResult> hashSums;

        static void Main(string[] args)
        {
            if(args.Length == 0)
                args = InitArgs();

            TryMainAction(args);
        }

        private static string[] InitArgs()
        {
            Console.WriteLine("Start program without args.");
            Console.Write("Enter Dirs\\Files (separated by space): ");
            string[] args = Console.ReadLine().Split(' ');
            return args;
        }

        private static void TryMainAction(string[] args)
        {
            filePathsStash = new MyConcurrentQueue<string>();
            hashSums = new MyConcurrentQueue<HashFunctionResult>();
            var connection = new SqlDbConnection(GetConnectionStringForLocalDB());
            try
            {
                var collector = new ThreadFileCollector(true);
                collector.ExecuteToFrom(filePathsStash, args);

                var calculator = new ThreadHashSumCalculator(2);
                calculator.StartComputingFromTo(collector, filePathsStash, hashSums);

                var writer = new ThreadWriter();
                
                connection.Open();
                connection.TryCreateTable();
                writer.StartFromTo(calculator, hashSums, connection);

                collector.Join();
                calculator.Join();
                writer.Join();
            }
            catch (SqlException e)
            {
                Console.WriteLine($"ERROR MESSAGE: {e.Message}\n" +
                    $"STACKTRACE: {e.StackTrace}");
            }
            finally
            {
                connection.Close();
                Console.WriteLine("End of programm.");
                Console.ReadKey();
            }
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
    }
}
