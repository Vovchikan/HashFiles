using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace HashFiles
{
    class MySqlServerHelper
    {
        private SqlConnectionStringBuilder builder;

        #region Constructors
        public MySqlServerHelper()
        {
            builder = new SqlConnectionStringBuilder();
            builder.DataSource = "(local)";
            builder.InitialCatalog = "HashHash";
            builder.IntegratedSecurity = true;
        }

        public MySqlServerHelper(string dataSource, string initialCatalog,
            bool integratedSecurity) : this()
        {
            builder.DataSource = dataSource;
            builder.InitialCatalog = initialCatalog;
            builder.IntegratedSecurity = integratedSecurity;
        }

        public MySqlServerHelper(string connectionString)
        {
            builder = new SqlConnectionStringBuilder(connectionString);
        }
        #endregion

        public SqlConnection NewConnection()
        {
            return new SqlConnection(builder.ConnectionString);
        }

        public void TryCreateTable(SqlConnection sqlCon, bool open=true)
        {
            if (!open)
                sqlCon.Open();
            try
            {
                using(SqlCommand command = new SqlCommand(
                    "CREATE TABLE HASHRESULTS (FileName TEXT, HashSum TEXT, " +
                    "Errors Text)", sqlCon))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                Console.WriteLine("Table not created.");
            }
        }

        public void AddHashSum(SqlConnection sqlCon, string fileName, 
            string hashSum, string errors, bool open=true)
        {
            if (!open) sqlCon.Open();
            try
            {
                using (SqlCommand command = new SqlCommand(
                    "INSERT INTO HASHRESULTS (FileName, HashSum, Errors) " +
                    "VALUES(@FileName, @HashSum, @Errors)", sqlCon))
                {
                    command.Parameters.Add(new SqlParameter("@FileName", fileName));
                    command.Parameters.Add(new SqlParameter("@HashSum", hashSum));
                    command.Parameters.Add(new SqlParameter("@Errors", errors));
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                Console.WriteLine("Count not insert.");
            }
        }
        
        public void AddHashSum(SqlConnection sqlCon, params string[] parametrs)
        {
            if(parametrs.Length == 3)
            {
                string fileName = parametrs[0];
                string hashSum = parametrs[1];
                string errors = parametrs[2];
                this.AddHashSum(sqlCon, fileName, hashSum, errors);
            }
        }
    }
}
