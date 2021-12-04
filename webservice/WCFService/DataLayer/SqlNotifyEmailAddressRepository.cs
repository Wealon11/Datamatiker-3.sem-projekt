using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WCFService.DataLayer
{
    public class SqlNotifyEmailAddressRepository : INotifyEmailAddressRepository
    {
        private readonly string _connectionString;

        public SqlNotifyEmailAddressRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<string> GetAll()
        {
            using (var conn = new SqlConnection(_connectionString))
            {
                var cmd = conn.CreateCommand();
                cmd.CommandText = @"SELECT Mail FROM MailDB";

                var addresses = new List<string>();
                conn.Open();
                var r = cmd.ExecuteReader();
                while (r.Read())
                {
                    addresses.Add(r.GetString(0));
                }

                return addresses;
            }
        }
    }
}