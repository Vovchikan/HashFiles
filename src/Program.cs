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

            //Options mainOptions = null;
            //Parser.Default.ParseArguments<BdOptions, FileOptions, ConsoleOptions>(args)
            //    .WithParsed<ConsoleOptions>(opt => mainOptions = opt)
            //    .WithParsed<BdOptions>(opt => throw new NotImplementedException())
            //    .WithParsed<FileOptions>(opt => throw new NotImplementedException());
            Console.WriteLine("End of programm.");
            Console.ReadKey();
        }

        private static string[] InitArgs()
        {
            Console.WriteLine("Start program without args.");
            Console.Write("Enter Dirs\\Files (separated by space): ");
            string[] args = ("console --paths " + Console.ReadLine()).Split(' '); 
            // todo использовать свойсво Default у Verbs
            return args;
        }
    }
}
