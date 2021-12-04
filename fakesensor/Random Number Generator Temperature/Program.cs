using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Random_Number_Generator_Temperature
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new HttpClient();
           
            while (true)
            {
                Random rnd = new Random();
               int temp = rnd.Next(31, 46);
                Console.WriteLine(temp);

                StringContent content = new StringContent(JsonConvert.SerializeObject(new {temps = temp}));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var response = client.PostAsync("http://localhost:57891/Service1.svc/temperatur/", content).Result;
                response.EnsureSuccessStatusCode();

                System.Threading.Thread.Sleep(10000);

                
            }

        }
    }
}
