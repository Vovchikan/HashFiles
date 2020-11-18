using System;
using System.Collections;
using System.Collections.Generic;
using HashFiles.src.options;
using HashFiles.src.threadWriters;
using NUnit.Framework;

namespace NUnitTests
{
    public class DataOptionsTestCases
    {
        private static IEnumerable<string> paths = new string[] { "./" };
        private static bool defRecursive = false;
        private static bool verbose = false;
        private static bool hideMode = false;
        private static bool overwrite = false;

        public static IEnumerable TestCasesConnectionFabrica
        {
            get
            {
                yield return new TestCaseData(CreateBdOptions()).Returns(typeof(ConnectionWithSqlDb));
                yield return new TestCaseData(CreateConsoleOptions()).Returns(typeof(ConnectionWithConsole));
                yield return new TestCaseData(CreateFileOptions()).Returns(typeof(ConnectionWithFile));
            }
        }

        private static OptionsForSqlDb CreateBdOptions()
        {
            return new OptionsForSqlDb(OptionsForSqlDb.defaultConfigeFilePath,
                    OptionsForSqlDb.defaultTableName, paths, defRecursive, Options.defaultThreadsCount, verbose);
        }

        private static OptionsForConsole CreateConsoleOptions()
        {
            return new OptionsForConsole(hideMode, paths, defRecursive, Options.defaultThreadsCount, verbose);
        }

        private static OptionsForFile CreateFileOptions()
        {
            return new OptionsForFile(OptionsForFile.defaultOutputDirPath, OptionsForFile.defaultFileName, overwrite,
                paths, defRecursive, Options.defaultThreadsCount, verbose);
        }

        public static IEnumerable TestCasesVerbs
        {
            get
            {
                yield return new TestCaseData("console").Returns(typeof(OptionsForConsole));
                yield return new TestCaseData("bd").Returns(typeof(OptionsForSqlDb));
                yield return new TestCaseData("file").Returns(typeof(OptionsForFile));
            }
        }
    }

}
