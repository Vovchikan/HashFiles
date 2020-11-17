using System;
using CommandLine;
using NUnit.Framework;
using HashFiles.src.options;

namespace NUnitTests
{
    [TestFixture]
    public class TestOptions
    {
        readonly string tempPaths = "d:/Pictures ./ ../../image.png";

        [TestCaseSource(typeof(DataOptionsTestCases), "TestCasesVerbs")]
        public Type TestVerbs(string verb)
        {
            string[] args = ($"{verb} " +
                $"--paths {tempPaths}").Split();
            Options options = null;
            Parser.Default.ParseArguments<OptionsForSqlDb, OptionsForFile, OptionsForConsole>(args)
                .WithParsed<Options>(opt => options = opt);
            Assert.IsNotNull(options);
            Assert.AreEqual(tempPaths.Split(), options.Paths);
            return options.GetType();
        }

        [Test]
        public void TestRecursiveOption()
        {
            string[] args = $"-r --paths {tempPaths}".Split();
            Options opt = null;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o => opt = o);
            Assert.IsNotNull(opt);
            Assert.AreEqual(tempPaths.Split(), opt.Paths);
            Assert.IsTrue(opt.Recursive);
        }

        [Test]
        public void TestPathsOption(
            [Values("-p", "--paths")] string optionPath)
        {
            string[] args = $"{optionPath} {tempPaths}".Split();
            Options opt = null;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o => opt = o);
            Assert.IsNotNull(opt);
            Assert.AreEqual(tempPaths, String.Join(" ", opt.Paths));
        }

        [Test]
        public void TestThreadsCountOption()
        {
            var option = "--threads";
            int value = 3;
            string[] args = ($"{option} {value} " +
                $"-p {tempPaths}").Split();
            Options opt = null;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o => opt = o);
            Assert.IsNotNull(opt);
            Assert.AreEqual(value, opt.ThreadsCount);
        }

        [Test]
        public void TestVerboseOption(
            [Values("-v","--verbose")] string option)
        {
            string[] args = ($"{option} " +
                $"-p {tempPaths}").Split();
            Options opt = null;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o => opt = o);
            Assert.IsNotNull(opt);
            Assert.IsTrue(opt.Verbose);
        }

        [Test]
        public void TestDefaultValuesOfFields()
        {
            string[] args = ($"--paths {tempPaths}").Split();
            Options opt = null;
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o => opt = o);
            Assert.NotNull(opt);
            Assert.IsFalse(opt.Recursive);
            Assert.AreEqual(Options.defaultThreadsCount, opt.ThreadsCount);
            Assert.IsFalse(opt.Verbose);
        }
    }
}
