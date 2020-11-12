using System;
using CommandLine;
using HashFiles.src.options;

namespace HashFiles
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
                args = InitArgs();

            var myAction = new MainAction();
            Parser.Default.ParseArguments<OptionsForSqlDb, OptionsForFile, OptionsForConsole>(args)
                .MapResult(
                (OptionsForSqlDb options) => myAction.TryMainAction(options),
                (OptionsForFile options) => myAction.TryMainAction(options),
                (OptionsForConsole options) => myAction.TryMainAction(options),
                error => 1);
        }

        private static string[] InitArgs()
        {
            Console.WriteLine("Start program without args.");
            Console.Write("Enter Dirs\\Files (separated by space): ");
            string[] args = ("console --paths " + Console.ReadLine()).Split(' ');
            return args;
        }
    }
}
