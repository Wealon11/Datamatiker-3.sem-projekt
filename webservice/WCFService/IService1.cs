using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;
using WCFService.Models;

namespace WCFService
{
    [ServiceContract]
    public interface IService1
    {
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "temperatur/")]
        Task InsertTemperature(IncomingTemperature temperature);

        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "encrypt/")]
        int ValidateCredentials(UserInfo userinfo);

        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "user/")]
        int CreateUserLogIn(UserInfo userinfo);

        [WebInvoke(Method = "PUT", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "maxtemp/")]
        void UpdateMaxTemp(MaxTemperatur maxTemperatur);

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "maxtemp/")]
        int GetMaxTemp();

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
         BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "alltemps/?start={start}&end={end}")]
        IList<ResponseTemperatur> GetTemperatures(string start, string end);

        [WebInvoke(Method = "POST", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "mail/")]
        string CreateEmail(Mail mail);

        [WebInvoke(Method = "GET", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "mail/")]
        IList<Mail> Getmail();

        [WebInvoke(Method = "DELETE", ResponseFormat = WebMessageFormat.Json,
            BodyStyle = WebMessageBodyStyle.Bare, UriTemplate = "mail/?addr={addr}")]
        string DeleteEmail(string addr);
    }
}
