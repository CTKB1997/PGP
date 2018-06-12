using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using ProductAPI_Client.Key;
/// <summary>
/// Login: aaa - 123
/// Login: abc - 123
/// </summary>
namespace ProductAPI_Client
{
    public class Service
    {
        private static HttpClient client = new HttpClient();

        private static string uri = "http://localhost:53727/api/products/";

        private static string baseURI = "http://localhost:53727/api/acc/";

        private int Menu()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine(" * *****************************************************");
                    Console.WriteLine("*** Welcome ***");
                    Console.WriteLine("1. Show all products");
                    Console.WriteLine("2. Add new product");
                    Console.WriteLine("3. Update product");
                    Console.WriteLine("4. Delete product");
                    Console.WriteLine("5. Exit");
                    Console.Write("Your choice: ");
                    int ch = Int16.Parse(Console.ReadLine());
                    if (ch > 0 && ch < 6) return ch;
                    else Console.WriteLine("Choice again!!!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Choice again!!!");
                }
            }
        }

        private void show()
        {
            string newUri = uri + "get-all-products";
            HttpResponseMessage resp = client.GetAsync(newUri).Result;
            //kiem tra co loi hay khong ?
            //resp.EnsureSuccessStatusCode();
            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(resp.Content.ToString());
                List<Product> proList = resp.Content.ReadAsAsync<List<Product>>().Result;
                foreach (Product p in proList)
                {
                    Console.WriteLine("****************************");
                    Console.WriteLine("Product ID: " + p.ProductID);
                    Console.WriteLine("Product Name: " + p.ProductName);
                    Console.WriteLine("Product Price: " + p.UnitPrice);
                    Console.WriteLine("Product Quantity: " + p.Quantity);
                }
            }
        }

        private void add()
        {
            Product pro = new Product();
            Console.WriteLine("********* ADD ********");
            /*Console.Write("Enter Product ID: ");
            pro.ProductID = Int16.Parse(Console.ReadLine());
            string newURI = uri + "get-product/" + pro.ProductID.ToString();
            if (client.GetAsync(newURI).Result.StatusCode == System.Net.HttpStatusCode.OK) {
                Console.WriteLine("ID existed!!!");

                return;
            }*/

            Console.Write("Enter Product Name: ");
            pro.ProductName = Console.ReadLine();

            Console.Write("Enter Product Price: ");
            pro.UnitPrice = Int16.Parse(Console.ReadLine());

            Console.Write("Enter Product Quantity: ");
            pro.Quantity = Int16.Parse(Console.ReadLine());
            
            #region get-public-key
            string newURI = baseURI + "get-public-key";
            //HttpResponseMessage response = await client.GetAsync(newURI);
            var response = client.GetStringAsync(newURI).Result.ToString();
            Obj obj = JsonConvert.DeserializeObject<Obj>(response);
            PgpEncryptionKeys encryptionKeys = new PgpEncryptionKeys(obj.file);
            #endregion

            #region add API
            newURI = uri + "create-product/";

            PgpEncrypt encrypter = new PgpEncrypt(encryptionKeys);
            string s = JsonConvert.SerializeObject(pro);

            System.IO.MemoryStream output = new System.IO.MemoryStream();
            encrypter.EncryptAndSign(output, s);
            output.Close();

            KeyObj newObj = new KeyObj();
            newObj.kByte = output.ToArray();

            HttpResponseMessage resp = client.PostAsJsonAsync(newURI, newObj).Result;
            #endregion

            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Add successful!!!");
            }
            else
            {
                Console.WriteLine("Add fail!!!");
            }
        }
        

        private void update()
        {
            Product pro = new Product();
            Console.WriteLine("********** UPDATE *********");
            Console.Write("Enter Product ID: ");
            pro.ProductID = Int16.Parse(Console.ReadLine());
            string newURI = uri + "get-product/" + pro.ProductID.ToString();
            //Console.WriteLine(newURI);
            if (client.GetAsync(newURI).Result.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine("ID not found!!");
                return;
            }


            Console.Write("Enter New Product Name: ");
            pro.ProductName = Console.ReadLine();
            Console.Write("Enter New Product Price: ");
            pro.UnitPrice = Int16.Parse(Console.ReadLine());
            Console.Write("Enter New Product Quantity: ");
            pro.Quantity = Int16.Parse(Console.ReadLine());


            #region get-public-key
            newURI = baseURI + "get-public-key";
            //HttpResponseMessage response = await client.GetAsync(newURI);
            var response = client.GetStringAsync(newURI).Result.ToString();
            Obj obj = JsonConvert.DeserializeObject<Obj>(response);
            PgpEncryptionKeys encryptionKeys = new PgpEncryptionKeys(obj.file);
            #endregion

            #region update API
            newURI = uri + "update-product/" + pro.ProductID.ToString();

            PgpEncrypt encrypter = new PgpEncrypt(encryptionKeys);
            string s = JsonConvert.SerializeObject(pro);

            System.IO.MemoryStream output = new System.IO.MemoryStream();
            encrypter.EncryptAndSign(output, s);
            output.Close();

            //MemoryStream ms = new MemoryStream();
            //PGPHandler.GetInstance().EncryptAndSign(Encoding.UTF8.GetBytes(request), ms);
            //String encryptedData = Encoding.UTF8.GetString(ms.ToArray());

            KeyObj newObj = new KeyObj();
            newObj.kByte = output.ToArray();

            HttpResponseMessage resp = client.PutAsJsonAsync(newURI, newObj).Result;
            #endregion


            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Update successful!!!");
            }
            else
            {
                Console.WriteLine("Update fail!!!");
            }
        }

        private void delete()
        {
            Console.WriteLine("********** DELETE *********");
            Console.Write("Enter Product ID: ");

            int id = Int16.Parse(Console.ReadLine());
            string newUri = uri + "get-product/" + id.ToString();
            if (client.GetAsync(newUri).Result.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Console.WriteLine("ID Not Found");
                return;
            }
            newUri = uri + "delete-product/" + id.ToString();
            HttpResponseMessage resp = client.DeleteAsync(newUri).Result;
            //Console.WriteLine(newUri);
            //Console.WriteLine(resp.StatusCode);
            if (resp.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine("Delete successful!!!");
            }
            else
            {
                Console.WriteLine("Delete fail!!!");
            }
        }

        public void Start()
        {
            //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "kGsAgNeZ7zoyT2JyGKqnlx0W6yuO352fkfIqiZnX9R6LHfGLsJV--pgzEUtGOkne8SdjP1vwg9sYtqcmO5Gp8nbA-YfV7haZbu1oVYmEoEkL2bFkztIspTPKBbh0orYkv9xB1wZIZYLRO721pybkKpWjaT1Cu4xo_87Rs1l1pGab4GCD5GSESMuLfrljbX4NuR8DbfG0i4j5preFgjxbfnmyeQ1XLqp1HxHIp6qNnbHCbjnPskq1LOcL4LobigukV5BQ3do3nzPr3yqyeNQ7ZrM6iA_QqbzTRrM5WHoTMYTuM03iBdZLQjA2zRHozitPsyXVks5QCosm0jEQk1UrtWKXmUFIzFwUtRfAR1Vz-mW4VChZdGppEMa8I92YS6ctIz_DG6PE3-wK6GTXQNzF9aj1H0akvUbnlGVTxnA2NimxeIixq0l4Bn25xGW7Cyj3RLcyu6esbF41sqIU_DR3PsQMTsrPvHB_tKxANJsEoiEaP437alypYRcZYSj3pDax");
            while (true)
            {
                int ch = Menu();
                switch (ch)
                {
                    case 1: show(); break;
                    case 2: add(); break;
                    case 3: update(); break;
                    case 4: delete(); break;
                    default: return;
                }
                Console.WriteLine("Click enter to continue...");
                Console.ReadLine();
            }
        }

        public Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public string GenerateStringFromStream(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            return text;
        }

    }

    public class JsonContent : HttpContent
    {
        private readonly MemoryStream _Stream = new MemoryStream();
        public JsonContent(object value)
        {

            Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var jw = new JsonTextWriter(new StreamWriter(_Stream));
            jw.Formatting = Formatting.Indented;
            var serializer = new JsonSerializer();
            serializer.Serialize(jw, value);
            jw.Flush();
            _Stream.Position = 0;

        }

        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            throw new NotImplementedException();
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _Stream.Length;
            return true;
        }
    }
}
