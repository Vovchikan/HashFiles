using HashFiles.src.options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace HashFiles.src.threadWriters
{
    public class ConnectionWithSqlDb : ConnectionWith
    {
        private readonly string tableDefaultName = "HASHRESULTS";
        private SqlConnection connection;
        private string tableName = "";
        private OptionsForSqlDb options;

        public ConnectionWithSqlDb(string connectionString)
        {
            connection = new SqlConnection(connectionString);
        }

        public ConnectionWithSqlDb(OptionsForSqlDb options)
        {
            this.options = options;
        }

        private string GetConnectionStringFromOptions()
        {
            string connectiongString = null;
            FileInfo fi = new FileInfo(options.ConfigeFilePath);
            try
            {
                if (File.Exists(fi.FullName))
                    using (var sw = new StreamReader(fi.FullName))
                    {
                        connectiongString = sw.ReadToEnd();
                        if (String.IsNullOrEmpty(connectiongString))
                            throw new ArgumentException($"Wrong file!\n" +
                                $"{fi.Name}, is empty!");
                    }
                else
                {
                    throw new FileNotFoundException($"File {fi.FullName} not found.");
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("\n"+e.Message+"\n");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("\n"+e.Message);
                Console.WriteLine("Use help for more info.\n");
            }
            return connectiongString;
        }

        public override void Close()
        {
            connection?.Close();
            connection?.Dispose();
        }

        public override void PrepareForWriting()
        {
            string connectionString = GetConnectionStringFromOptions();
            this.connection = new SqlConnection(connectionString);
            connection.Open();
            TryCreateTable();
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
            if (options.Verbose)
            {
                Console.WriteLine($"Trying to send new data to {tableName.ToUpper()}:\n" +
                        $"DATA: {data}");
            }
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
                if(options.Verbose)
                    Console.WriteLine(e.Message);
            }
            catch (Exception e)
            {
                if(options.Verbose)
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
