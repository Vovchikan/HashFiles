using System;
using System.Collections.Generic;
using CommandLine;
using CommandLine.Text;

namespace HashFiles.src.options
{
    public class Options
    {
        private const string version = "0.1.0";
        private readonly string heading = $"HashFiles {version}";
        private const string copyright = "Copyright (c) 2020 https://github.com/Vovchikan";
        private readonly bool recursive;
        private readonly IEnumerable<string> paths;
        private readonly int threadsCount;
        private readonly bool verbose;
        public const int defaultThreadsCount = 2;

        public Options(IEnumerable<string> paths, bool recursive, int threadsCount, bool verbose)
        {
            this.recursive = recursive;
            this.paths = paths;
            this.threadsCount = threadsCount;
            this.verbose = verbose;
        }

        [Option('p', "paths", Required = true, 
            HelpText = "Paths to files\\dirs for hash sums calculation.")]
        public virtual IEnumerable<String> Paths { get { return paths; } }

        [Option('r', Default = false,
            HelpText = "Turn on recursive calculation from directories.")]
        public virtual bool Recursive { get { return recursive; } }

        [Option("threads", Default = defaultThreadsCount,
            HelpText = "Count of threads for calculation hash sum.")]
        public int ThreadsCount { get { return threadsCount; } }

        [Option('v', "verbose", Default = false)]
        public bool Verbose { get { return verbose; } }

        public void DisplayHelp<T>(ParserResult<T> result)
        {
            var helpText = HelpText.AutoBuild(result, h =>
            {
                h.AdditionalNewLineAfterOption = false;
                h.Heading = heading;
                h.Copyright = copyright;
                return HelpText.DefaultParsingErrorsHandler(result, h);
            }, e => e);
            Console.WriteLine(helpText);
        }
    }
}
