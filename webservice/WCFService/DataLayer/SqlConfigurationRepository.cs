using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WCFService.DataLayer
{
    public class SqlConfigurationRepository : IConfigurationRepository
    {
        private readonly string _connectionString;

        public SqlConfigurationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public string Get(string key)
        {
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand())
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT ConfValue FROM Configuration WHERE ConfKey = @key";
                cmd.Parameters.AddWithValue("@key", key);
                var confValue = (string)cmd.ExecuteScalar();
                if (confValue == "")
                {
                    confValue = null;
                }
                return confValue;
            }
        }

        public void Set(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{nameof(key)} and {nameof(value)} parameters must be non-null and non-empty");
            }

            using (var conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = conn.CreateCommand();
                var transaction = conn.BeginTransaction();
                cmd.Connection = conn;
                cmd.Transaction = transaction;

                try
                {
                    cmd.CommandText = "SELECT 1 FROM Configuration WHERE ConfKey = @key";
                    cmd.Parameters.AddWithValue("@key", key);
                    var configExists = cmd.ExecuteScalar() == null;
                    if (configExists)
                    {
                        cmd.CommandText = "INSERT INTO Configuration VALUES (@key, @value)";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@key", key);
                        cmd.Parameters.AddWithValue("@value", value);
                        var rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected != 1)
                        {
                            throw new Exception($"rows affected by insert into configuration != 1: {rowsAffected}");
                        }
                    }
                    else
                    {
                        cmd.CommandText = "UPDATE Configuration SET ConfValue = @value WHERE ConfKey = @key";
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@key", key);
                        cmd.Parameters.AddWithValue("@value", value);
                        var rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected != 1)
                        {
                            throw new Exception($"rows affected by insert into configuration != 1: {rowsAffected}");
                        }
                    }
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}