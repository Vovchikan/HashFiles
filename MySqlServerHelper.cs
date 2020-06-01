using System;
using System.Data;
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
                    "Errors NVARCHAR(MAX))", sqlCon))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                Console.WriteLine("Table not created.");
            }
        }

        
        public void AddHashSum(SqlConnection sqlCon, HashFunctionResult result)
        {
            if(showAddingToConsole) Console.WriteLine("Добаваляем: " + result.ToString());
            TryInsertNewData(sqlCon, result);
        }

        public void TryInsertNewData(SqlConnection sqlCon, HashFunctionResult result)
        {
            try
            {
                using (SqlCommand command = new SqlCommand(
                    "INSERT INTO HASHRESULTS (FileName, HashSum, Errors) " +
                    "VALUES(@FileName, @HashSum, @Errors)", sqlCon))
                {
                    command.Parameters.Add(new SqlParameter("@FileName", result.filePath));
                    command.Parameters.Add(new SqlParameter("@HashSum", result.hashSum));
                    command.Parameters.Add(new SqlParameter("@Errors", SqlDbType.NVarChar) { Value = result.error.ErrorMessage });
                    command.ExecuteNonQuery();
                }
            }
            catch
            {
                throw new Exception("Не удалось добавить данные: " + result.ToString());
            }
        }

        public bool CheckContains(SqlConnection sqlCon, HashFunctionResult result)
        {
            try
            {
                using (SqlCommand command = new SqlCommand(
                    "SELECT * FROM HASHRESULTS " +
                    "WHERE FileName = @FileName AND HashSum = @HashSum", sqlCon))
                {
                    command.Parameters.AddWithValue("@FileName", result.filePath);
                    command.Parameters.AddWithValue("@HashSum", result.hashSum);
                    using (SqlDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                        {
                            reader.Close();
                            if (showDublicate) Console.WriteLine("Дубликат: {0} {1}", result.filePath, result.hashSum);
                            return true;
                        }
                }
            }
            catch
            {
                throw new Exception(String.Format("Ошибка в поиске данных: path = {0}, hashSum = {1}", result.filePath, result.hashSum));
            }
            return false;
        }
    }
}
