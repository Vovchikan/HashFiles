using System;
using System.Data.SqlClient;

namespace HashFiles
{
    class MySqlServerHelper
    {
        private SqlConnectionStringBuilder builder;
        private bool showAddingToConsole = false;
        private bool showDublicate = true;

        #region Constructors

        public MySqlServerHelper(string dataSource, string initialCatalog,
            bool integratedSecurity)
        {
            builder = new SqlConnectionStringBuilder();
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

        public void TryCreateTable(SqlConnection sqlCon)
        {
            try
            {
                using(SqlCommand command = new SqlCommand(
                    "CREATE TABLE HASHRESULTS (FileName VARCHAR(MAX), HashSum VARCHAR(MAX), " +
                    "Errors VARCHAR(MAX))", sqlCon))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                Console.WriteLine("Table not created.");
            }
        }

        
        public void AddHashSum(SqlConnection sqlCon, params string[] parametrs)
        {
            if(showAddingToConsole) Console.WriteLine("Добаваляем: " + string.Join(" ", parametrs));
            if(parametrs.Length == 3)
            {
                string fileName = parametrs[0];
                string hashSum = parametrs[1];
                string errors = parametrs[2];
                this.AddHashSum(sqlCon, fileName, hashSum, errors);
            }
        }

        public void AddHashSum(SqlConnection sqlCon, string fileName,
            string hashSum, string errors)
        {
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

        public bool CheckContains(SqlConnection sqlCon, params string[] parametrs)
        {
            if (parametrs.Length == 3)
            {
                try
                {
                    string fileName = parametrs[0];
                    string hashSum = parametrs[1];
                    using (SqlCommand command = new SqlCommand(
                        "SELECT * FROM HASHRESULTS " +
                        "WHERE FileName = @FileName AND HashSum = @HashSum", sqlCon))
                    {
                        command.Parameters.AddWithValue("@FileName", fileName);
                        command.Parameters.AddWithValue("@HashSum", hashSum);
                        using (SqlDataReader reader = command.ExecuteReader())
                            while (reader.Read())
                            {
                                reader.Close();
                                if(showDublicate) Console.WriteLine("Дубликат: {0} {1}", fileName, hashSum);
                                return true;
                            }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("filename = {0}, hashSum = {1}", parametrs[0], parametrs[1]);
                    throw ;
                }
            }
            return false;
        }
    }
}
