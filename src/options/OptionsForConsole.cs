using CommandLine;
using System.Collections.Generic;

namespace HashFiles.src.options
{
    [Verb("console", HelpText = "Count hash sum of files and print results.")]
    public class OptionsForConsole : Options
    {
        public OptionsForConsole(IEnumerable<string> paths, 
            bool recursive, int threadsCount, bool verbose) : base(paths, recursive, threadsCount, verbose)
        {

        }
    }
}
