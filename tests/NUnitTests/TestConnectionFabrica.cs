using System;
using NUnit.Framework;
using HashFiles.src.options;
using HashFiles.src.threadWriters;

namespace NUnitTests
{
    [TestFixture]
    public class TestConnectionFabrica
    {
        [TestCaseSource(typeof(DataOptionsTestCases), "TestCasesConnectionFabrica")]
        public Type TestFabricaCreation(Options opt)
        {
            var fabrica = new ConnectionFabrica();
            var connection = fabrica.CreateConnection(opt);
            return connection.GetType();
        }
    }
}
