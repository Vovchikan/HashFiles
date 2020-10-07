using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TestDataBaseWorker
{
    [TestFixture]
    public class TestSQLConnection
    {
        [Test]
        public void LocalSqlServerConnection()
        {
            using (var sc = new SqlConnection(@"Server=(localdb)\MSSQLLocalDB;"))
            {
                sc.Open();
                Assert.AreEqual(sc.State, System.Data.ConnectionState.Open);
            }
        }

        [Test]
        public void FindDatabase1AtLocalSqlServer(
            [Values(relativeConnectionString)] String connectionString)
        {
            using (var sc = new SqlConnection(connectionString))
            {
                
                sc.Open();
                Assert.AreEqual(sc.State, System.Data.ConnectionState.Open);
            }
        }

        private const String relativeConnectionString = @"Data Source = (localdb)\MSSQLLocalDB;
                AttachDbFilename=|DataDirectory|\Database1.mdf;
                Integrated Security=True;Connect Timeout=30;";

    }
}
