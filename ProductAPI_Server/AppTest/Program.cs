using AppTest.Key;
using ProductAPI_Server.Models;
using System;
using System.IO;
using Newtonsoft.Json;

namespace AppTest
{
    class Program
    {
        private static string privateKeyPath = @"D:\Keys\PGPPrivateKey.asc";
        private static string pwd = "P@ll@m@lli";
        private static string directory = @"D:\Keys\";
        static void Main(string[] args)
        {

            Random rnd = new Random();
            int rndInt = rnd.Next(0, 20000000);
            Byte[] ss = System.IO.File.ReadAllBytes(@"D:\Keys\2018-06-08-13-49-18-OUTPUT-3284217.txt");
            //Console.WriteLine(ss);
            string fileName = directory + "2018-06-08-13-49-18-OUTPUT-3284217.txt";
            //string fileName = directory + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-OUTPUT-" + rndInt.ToString(), CultureInfo.InvariantCulture) + ".txt";
            System.IO.File.WriteAllBytes(fileName, ss);
            Console.WriteLine(fileName);
            //var item = JsonConvert.DeserializeObject<Product>(requestParam.ToString());
            //string item = JsonConvert.DeserializeObject<String>(requestParam.ToString());
            //PGPDecrypt.Decrypt("D:\\Keys\\EncryptData.txt", @"D:\Keys\PGPPrivateKey.asc", "P@ll@m@lli", "D:\\Keys\\OriginalText.txt");
            Stream output = PGPDecrypt.Decrypt(fileName, privateKeyPath, pwd);
            string later = GenerateStringFromStream(output);
            Console.WriteLine(later);
            var item = JsonConvert.DeserializeObject<Product>(later);

        }


        public static string GenerateStringFromStream(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            return text;
        }
    }
}
