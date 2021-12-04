using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WCFService.DataLayer
{
    public class SqlTemperatureRepository : ITemperatureRepository
    {
        private readonly string _connectionString;

        public SqlTemperatureRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertTemperature(int temperatur, string sensorID)
        {
            if (String.IsNullOrEmpty(sensorID))
            {
                throw new ArgumentException("Null or Empty", nameof(sensorID));
            }
            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand())

            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = "insert into TempDB (Temperatur, Dato, [Sensor ID]) values (@Temperatur, @Dato, @sid)";
                cmd.Parameters.AddWithValue("@Dato", DateTime.Now);
                cmd.Parameters.AddWithValue("@Temperatur", temperatur);
                cmd.Parameters.AddWithValue("@sid", sensorID);
                cmd.ExecuteNonQuery();

            }
        }

        public List<Temperature> GetTemperatures(string start, string end)
        {

            DateTime startTime;
            DateTime endTime;

            bool startSuccess = DateTime.TryParse(start, out startTime);
            bool endSuccess = DateTime.TryParse(end, out endTime);

            using (var conn = new SqlConnection(_connectionString))
            using (var cmd = new SqlCommand())
            {
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = "select [Id],[Dato],[Temperatur],[Sensor ID] from TempDB where Dato between @start and @end";


                cmd.Parameters.AddWithValue("@start", startTime);
                cmd.Parameters.AddWithValue("@end", endTime);

                var reader = cmd.ExecuteReader();
                var temperatures = new List<Temperature>();
                while (reader.Read())
                {
                    var id = reader.GetInt32(0);
                    var date = reader.GetDateTime(1);
                    var temp = reader.GetInt32(2);
                    var sensorID = reader.GetString(3);

                    var temperatur = new Temperature
                    {
                        ID = id,
                        Date = date,
                        temps = temp,
                        SensorID = sensorID
                    };

                    temperatures.Add(temperatur);
                }

                return temperatures;

            }
        }
    }
}