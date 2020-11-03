using System;
using CommandLine;
using NUnit.Framework;
using HashFiles.src.options;

namespace NUnitTests
{
    [TestFixture]
    public class TestOptionsForFile
    {
        readonly string tempPaths = "d:/Pictures ./ ../../image.png";

        [Test]
        public void TestOptionsForFileNameAndOutput(
            [Values("-n", "--name")] string optionName,
            [Values("-o", "--output")] string optionOutput)
        {
            string tempFileName = "output.txt";
            string tempOutputDirPath = "d:\\Data";
            string[] args = ($"file " +
                $"{optionName} {tempFileName} " +
                $"{optionOutput} {tempOutputDirPath} " +
                $"--paths {tempPaths}").Split();
            OptionsForFile fileOpt = null;
            Parser.Default.ParseArguments<OptionsForSqlDb, OptionsForConsole, OptionsForFile>(args)
                .WithParsed<OptionsForFile>(o => fileOpt = o);
            Assert.IsNotNull(fileOpt);
            Assert.AreEqual(tempPaths, String.Join(" ", fileOpt.Paths));
            Assert.AreEqual(tempFileName, fileOpt.FileName);
            Assert.AreEqual(tempOutputDirPath, fileOpt.OutputDirPath);
        }

        [Test]
        public void TestDefaultValuesFileOptions()
        {
            string[] args = ($"file --paths {tempPaths}").Split();
            OptionsForFile fileOpt = null;
            Parser.Default.ParseArguments<OptionsForFile>(args)
                .WithParsed<OptionsForFile>(o => fileOpt = o);
            Assert.NotNull(fileOpt);
            Assert.AreEqual(OptionsForFile.defaultOutputDirPath, fileOpt.OutputDirPath);
            Assert.AreEqual(OptionsForFile.defaultFileName, fileOpt.FileName);
        }
    }
}
