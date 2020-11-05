using System;
using System.Data.SqlClient;
using System.Linq;

using HashFiles.src.options;
using HashFiles.src.threadWriters;

namespace HashFiles
{
    public class MainAction
    {
        private MyConcurrentQueue<string> filePathsStash;
        private MyConcurrentQueue<HashFunctionResult> hashSums;
        private ThreadFileCollector collector;
        private ThreadHashSumCalculator calculator;
        private ThreadWriter writer;
        private ConnectionWith connection;

        public int TryMainAction(Options options)
        {
            InitializateFields(options);

            try
            {
                collector.ExecuteToFrom(filePathsStash, options.Paths.ToArray<string>());
                calculator.StartComputingFromTo(collector, filePathsStash, hashSums);

                connection.PrepareForWriting();
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
            }

            return 0;
        }

        private void InitializateFields(Options options)
        {
            filePathsStash = new MyConcurrentQueue<string>();
            hashSums = new MyConcurrentQueue<HashFunctionResult>();
            collector = new ThreadFileCollector(options.Recursive);
            calculator = new ThreadHashSumCalculator(options.ThreadsCount);
            writer = new ThreadWriter();

            var connectionFabrica = new ConnectionFabrica();
            connection = connectionFabrica.CreateConnection(options);
        }
    }
}
