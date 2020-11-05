using System;
using CommandLine;
using NUnit.Framework;
using HashFiles.src.options;

namespace NUnitTests
{
    [TestFixture]
    public class TestOptionsForSqlDb
    {
        readonly string tempPaths = "d:/Pictures ./ ../../image.png";

        [Test]
        public void TestDefaultValuesBdOptions()
        {
            string[] args = ($"bd --paths {tempPaths}").Split();
            OptionsForSqlDb bdopt = null;
            Parser.Default.ParseArguments<OptionsForSqlDb>(args)
                .WithParsed<OptionsForSqlDb>(o => bdopt = o);
            Assert.IsNotNull(bdopt);
            Assert.AreEqual(OptionsForSqlDb.defaultConfigeFilePath, bdopt.ConfigeFilePath);
        }

        [Test]
        public void TestOptionsForSqlDbConfig(
            [Values("-c", "--config")] string optionConfig)
        {
            string tempConfigeFilePath = "config.txt";
            string[] args = ($"bd " +
                $"{optionConfig} {tempConfigeFilePath} " +
                $"--paths {tempPaths}").Split();
            OptionsForSqlDb bdOpt = null;
            Parser.Default.ParseArguments<OptionsForSqlDb, OptionsForConsole, OptionsForFile>(args)
                .WithParsed<OptionsForSqlDb>(o => bdOpt = o);
            Assert.IsNotNull(bdOpt);
            Assert.AreEqual(tempPaths, String.Join(" ", bdOpt.Paths));
            Assert.AreEqual(tempConfigeFilePath, bdOpt.ConfigeFilePath);
        }
    }
}
