using System.Collections.Generic;
using CommandLine;

namespace HashFiles.src.options
{
    [Verb("console", HelpText = "Count hash sum of files and print results.")]
    public class OptionsForConsole : Options
    {
        private bool hide;

        public OptionsForConsole(bool hide, IEnumerable<string> paths, 
            bool recursive, int threadsCount, bool verbose) : base(paths, recursive, threadsCount, verbose)
        {
            this.hide = hide;
        }

        [Option('h',"hide", Default = false,
            HelpText = "Console doesn't print data")]
        public bool Hide { get { return hide; } }
    }
}
