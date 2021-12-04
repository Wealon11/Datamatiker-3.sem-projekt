using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using DevOne.Security.Cryptography.BCrypt;
using WCFService.DataLayer;
using WCFService.Models;
using WCFService.Services;

namespace WCFService
{
    public class Service1 : IService1
    {
        private readonly IAppConfig _appConfig;
        private readonly IConfigurationRepository _configurationRepository;
        private readonly ITemperatureRepository _temperatureRepository;

        private readonly IEnumerable<INotificationSender> _notifiers;

        public Service1() : this(null, null, null) { }

        public Service1(IAppConfig appConfig = null, IConfigurationRepository configRepo = null, ITemperatureRepository tempRepo = null, IEnumerable<INotificationSender> notifiers = null)
        {
            _appConfig = appConfig ?? AppConfig.Instance;
            _configurationRepository = configRepo ?? new SqlConfigurationRepository(_appConfig.ConnectionString);
            _temperatureRepository = tempRepo ?? new SqlTemperatureRepository(_appConfig.ConnectionString);
            _notifiers = notifiers ?? Global.NotificationSenders;
        }

        private enum UserReturns
        {
            UnhandledError = -1,
            UsernameIncorrect = 0,
            PasswordIncorrect = 1,
            LoginCorrect = 2
        }

        private async Task SendMessageToAllNotifiersAsync(string msg)
        {
            var tasks = new List<Task>();
            foreach (var notifier in _notifiers)
            {
                tasks.Add(notifier.SendMessageAsync(msg));
            }
            await Task.WhenAll(tasks);
        }

        public int ValidateCredentials(UserInfo userInfo)
        {
            const string CheckUser = "select * from UserInfo where Username = @Username";
            string user = "";
            string hashedpw = "";

            using (SqlConnection c = new SqlConnection(_appConfig.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand(CheckUser, c))
                {
                    c.Open();

                    com.Parameters.AddWithValue("@Username", userInfo.User);

                    SqlDataReader reader = com.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            user = reader.GetString(0);
                            hashedpw = reader.GetString(1);
                        }
                    }
                    reader.Close();
                }
            }

            if (String.IsNullOrWhiteSpace(user))
            {
                return (int)UserReturns.UsernameIncorrect;
            }

            if (user != userInfo.User)
            {
                return (int)UserReturns.UsernameIncorrect;
            }

            if (userInfo.User == user)
            {

                if (!BCryptHelper.CheckPassword(userInfo.Password, hashedpw))
                {
                    return (int)UserReturns.PasswordIncorrect;
                }

                //if (userInfo.Password != pw)
                //{

                //    return (int)UserReturns.PasswordIncorrect;
                //}

                return (int)UserReturns.LoginCorrect;
            }

            return (int)UserReturns.UnhandledError;
        }

        public int CreateUserLogIn(UserInfo userinfo)
        {
            const string insertTemp =
                "insert into UserInfo (Username, Password) values (@Username, @Password)";

            string hashedPassword = BCryptHelper.HashPassword(userinfo.Password, BCryptHelper.GenerateSalt());


            using (SqlConnection c = new SqlConnection(_appConfig.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand(insertTemp, c))
                {
                    c.Open();

                    com.Parameters.AddWithValue("@Username", userinfo.User);
                    com.Parameters.AddWithValue("@Password", hashedPassword);

                    int a = com.ExecuteNonQuery();

                    return a;
                }
            }
        }

        public async Task InsertTemperature(IncomingTemperature t)
        {
            _temperatureRepository.InsertTemperature(t.Temperature, t.SensorID);

            if (t.Temperature > GetMaxTemp())
            {
                await SendMessageToAllNotifiersAsync(t.Temperature.ToString());
            }
        }

        public void UpdateMaxTemp(MaxTemperatur maxTemperatur)
        {
            if (maxTemperatur == null)
            {
                throw new ArgumentNullException(nameof(maxTemperatur));
            }
            var isInt = int.TryParse(maxTemperatur.Maxtemps, out int _);
            if (!isInt)
            {
                throw new ArgumentException("Maxtemps value must be a valid integer", nameof(maxTemperatur));
            }
            _configurationRepository.Set("MaxTemp", maxTemperatur.Maxtemps);
        }

        public int GetMaxTemp()
        {
            var maxTemp = _configurationRepository.Get("MaxTemp");
            if (maxTemp == null)
            {
                return 0;
            }
            return int.Parse(maxTemp);
        }

        public IList<ResponseTemperatur> GetTemperatures(string start, string end)
        {
            var temperatures = _temperatureRepository.GetTemperatures(start, end);
            return temperatures.Select(t => new ResponseTemperatur(t)).ToList();
        }

        public string CreateEmail(Mail mail)
        {
            try
            {
                const string insert = "insert into MailDB (mail) values (@mail)";

                using (SqlConnection con = new SqlConnection(_appConfig.ConnectionString))
                {
                    using (SqlCommand com = new SqlCommand(insert, con))
                    {
                        con.Open();

                        com.Parameters.AddWithValue("@mail", mail.mail);

                        com.ExecuteNonQuery();
                        return "This Email has been inserted into the database";
                    }
                }
            }
            catch (SqlException)
            {
                return "This Email has already been taken";
            }
        }

        public IList<Mail> Getmail()
        {
            const string selectAll = "select * from MailDB";

            using (SqlConnection con = new SqlConnection(_appConfig.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand(selectAll, con))
                {
                    con.Open();

                    using (SqlDataReader reader = com.ExecuteReader())
                    {
                        List<Mail> mList = new List<Mail>();

                        while (reader.Read())
                        {
                            Mail m = ReadMail(reader);
                            mList.Add(m);
                        }

                        return mList;
                    }
                }
            }
        }

        private static Mail ReadMail(IDataRecord reader)
        {
            string Mail = reader.GetString(0);


            Mail m = new Mail
            {
                mail = Mail
            };
            return m;
        }

        public string DeleteEmail(string addr)
        {
            const string delete = "delete from MailDB where mail= @mail";

            using (SqlConnection con = new SqlConnection(_appConfig.ConnectionString))
            {
                using (SqlCommand com = new SqlCommand(delete, con))
                {
                    con.Open();

                    com.Parameters.AddWithValue("@mail", addr);

                    int row = com.ExecuteNonQuery();
                    if (row == 1)
                    {
                        return "This Email has been deleted from the database";
                    }

                    return "no Email could be found";
                }
            }
        }
    }
}

