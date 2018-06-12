using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ProductAPI_Client
{
    class Program
    {
        private static HttpClient client = new HttpClient();
        private static string uri = "http://localhost:53727/api/acc/login/";
        private static string baseURI = "http://localhost:53727";
        static void Main(string[] args)
        {
            while (true)
            {
                bool flag = false;
                try
                {
                    Console.WriteLine("*** Welcome ***");
                    Console.Write("Username: ");
                    Account acc = new Account();
                    acc.Id = Console.ReadLine();
                    Console.Write("Password: ");
                    acc.Password = Console.ReadLine();

                    //var a = client.GetAsync(uri + "api/tks").Result;
                    HttpResponseMessage a = client.PostAsJsonAsync(uri, acc).Result;
                    a.EnsureSuccessStatusCode();

                    //Console.WriteLine(a.Content.ReadAsStringAsync().Result);
                    //Console.WriteLine();
                    Console.WriteLine("Login successfull. Click enter to go to main menu!!!");
                    Console.ReadLine();
                    flag = true;
                }
                catch (Exception ex)
                {

                }
                if (flag)
                {
                    Service ser = new Service();
                    ser.Start();
                    break;
                }
            }
        }
    }
}
