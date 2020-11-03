using CommandLine;
using System.Collections.Generic;

namespace HashFiles.src.options
{
    [Verb("bd", HelpText = "Count hash sum of files and add results to bd.")]
    public class OptionsForSqlDb : Options
    {
        private readonly string configeFilePath;
        private readonly string tableName;
        public const string defaultTableName = "HASHRESULTS";
        public const string defaultConfigeFilePath = ".\\data\\connectionString.txt";
        public const string relativeConnectionString = @"Data Source = (localdb)\MSSQLLocalDB;
                AttachDbFilename=|DataDirectory|\Database1.mdf;
                Integrated Security=True;Connect Timeout=30;";

        public OptionsForSqlDb(string configeFilePath, string tableName,
            IEnumerable<string> paths, bool recursive, 
            int threadsCount, bool verbose) : base(paths, recursive, threadsCount, verbose)
        {
            this.configeFilePath = configeFilePath;
            this.tableName = tableName;
        }

        [Option('c', "config", Default = defaultConfigeFilePath,
            HelpText = "Path to file, which includes connecting string to data base.")]
        public string ConfigeFilePath { get { return configeFilePath; } }

        [Option('t', "table", Default = defaultTableName,
            HelpText = "Name of table in sql database. Default name - HASHRESULTS.")]
        public string TableName { get { return tableName; } }
    }
}
