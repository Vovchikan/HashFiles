using System;
using System.Data;
using System.Data.SqlClient;

namespace HashFiles.src.threadWriters
{
    public class SqlDbConnection : WriterConnection
    {
        private readonly SqlConnection connection;
        private readonly string tableDefaultName = "HASHRESULTS";
        private string tableName = "";

        public SqlDbConnection(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public override void Close()
        {
            connection?.Close();
            connection?.Dispose();
        }

        public override void Open()
        {
            connection.Open();
        }

        public void TryCreateTable()
        {
            if (String.IsNullOrEmpty(tableName))
                tableName = tableDefaultName;
            try
            {
                using (SqlCommand command = new SqlCommand(
                    $"CREATE TABLE {tableName} (FileName VARCHAR(MAX), HashSum VARCHAR(MAX), " +
                    "Errors NVARCHAR(MAX))", connection))
                {
                    //command.Parameters.Add(new SqlParameter("@Name", tableName));
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException sqlex)
            {
                Console.WriteLine($"Failed to create table \"{tableName}\"\n" +
                    $"MESSAGE ERROR: {sqlex.Message}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Unkown exception in \"TryCreateTable\" method!");
                throw e;
            }
        }

        public override void SendHashData(HashFunctionResult data)
        {
            Console.WriteLine("Trying to send new data:\n" +
                    $"TABLE NAME: {tableName}\n" +
                    $"DATA: {data}");
            TrySendHashData(data);
        }

        private void TrySendHashData(HashFunctionResult result)
        {
            try
            {
                FindDuplicateAndThrowExc(result);
                
                using (SqlCommand command = new SqlCommand(
                    $"INSERT INTO {tableName} (FileName, HashSum, Errors) " +
                    "VALUES(@FileName, @HashSum, @Errors)", connection))
                {
                    command.Parameters.Add(new SqlParameter("@FileName", result.filePath));
                    command.Parameters.Add(new SqlParameter("@HashSum", result.hashSum));
                    command.Parameters.Add(new SqlParameter("@Errors", SqlDbType.NVarChar) { 
                        Value = result.error.ErrorMessage });
                    command.ExecuteNonQuery();
                }
            }
            catch(DuplicateDataException e)
            {
                Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to send new data:\n" +
                    $"TABLE NAME: {tableName}\n"+
                    $"DATA: {result}");
                throw e;
            }
        }

        private void FindDuplicateAndThrowExc(HashFunctionResult result)
        {
            using (SqlCommand command = new SqlCommand(
                "SELECT * FROM HASHRESULTS " +
                "WHERE FileName = @FileName AND HashSum = @HashSum" +
                " AND Errors = @Errors", connection))
            {
                command.Parameters.AddWithValue("@FileName", result.filePath);
                command.Parameters.AddWithValue("@HashSum", result.hashSum);
                command.Parameters.AddWithValue("@Errors", result.error.ErrorMessage);
                using (SqlDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                    {
                        reader.Close();
                        throw new DuplicateDataException(result);
                    }
            }
        }
    }
}
