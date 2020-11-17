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
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<
                OptionsForConsole, OptionsForFile, OptionsForSqlDb>(args);

            parserResult.WithParsed<OptionsForConsole>(options => myAction.Start(options))
                .WithParsed<OptionsForFile>(options => myAction.Start(options))
                .WithParsed<OptionsForSqlDb>(options => myAction.Start(options))
                .WithNotParsed(errs => Options.DisplayHelp(parserResult, errs));
        }

        private static string[] InitArgs()
        {
            string[] args = { "--help" };
            return args;
        }
    }
}
