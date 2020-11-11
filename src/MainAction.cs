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
                calculator.StartComputingFromTo(filePathsStash, hashSums);

                connection.PrepareForWriting();
                writer.StartFromTo(hashSums, connection);

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
            if (options.Verbose)
                Console.WriteLine("End of programm.");
            return 0;
        }

        private void InitializateFields(Options options)
        {
            int collectorThreadsCount = 1;
            int writerThreadsCount = 1;
            filePathsStash = new MyConcurrentQueue<string>(collectorThreadsCount, options.ThreadsCount);
            hashSums = new MyConcurrentQueue<HashFunctionResult>(options.ThreadsCount, writerThreadsCount);
            collector = new ThreadFileCollector(options.Recursive);
            calculator = new ThreadHashSumCalculator(options.ThreadsCount, options.Verbose);
            writer = new ThreadWriter(options.Verbose);

            var connectionFabrica = new ConnectionFabrica();
            connection = connectionFabrica.CreateConnection(options);
        }
    }
}
