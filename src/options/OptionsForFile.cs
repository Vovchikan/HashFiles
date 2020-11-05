using CommandLine;
using System.Collections.Generic;

namespace HashFiles.src.options
{
    [Verb("file", HelpText = "Count hash sum of files and write results in file.")]
    public class OptionsForFile : Options
    {
        private readonly string outputDirPath;
        private readonly string fileName;
        public const string defaultOutputDirPath = ".\\data";
        public const string defaultFileName = "output.txt";

        public OptionsForFile( string outputDirPath, string fileName,
            IEnumerable<string> paths, bool recursive, 
            int threadsCount, bool verbose) : base(paths, recursive, threadsCount, verbose)
        {
            this.outputDirPath = outputDirPath;
            this.fileName = fileName;
        }

        [Option('o', "output", Default = defaultOutputDirPath,
            HelpText = "Path to directory, where output files will be created.")]
        public string OutputDirPath { get { return outputDirPath; } }

        [Option('n', "name", Default = defaultFileName,
            HelpText = "Name of output file. For ex. - \"output.txt\".")]
        public string FileName { get { return fileName; } }
    }
}
