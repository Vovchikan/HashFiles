using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashFiles
{
    public class DataStore
    {
        private MySqlServerHelper sqlHelper;
        private bool hasConnection = false;
        public DataStore()
        {
            sqlHelper = new MySqlServerHelper(GetConnectionStringForLocalDB());
        }

        private static String GetConnectionStringForLocalDB()
        {
            return @"Data Source = (localdb)\MSSQLLocalDB;
                AttachDbFilename=D:\Programming\C#\Github-portfolio\HashFiles\Database1.mdf;
                Integrated Security=True;Connect Timeout=30;";
        }

        public void AddDataElement(HashFunctionResult element)
        {
            if (!hasConnection)
            {
                PrepareConnection();
                hasConnection = true;
            }
            sqlHelper.AddHashSum(element);
        }

        private void PrepareConnection()
        {
            sqlHelper.NewConnection();
            sqlHelper.OpenConnection();
        }

        public void DeleteConnection()
        {
            sqlHelper.CloseAndDisposeConnection();
        }
    }
}
