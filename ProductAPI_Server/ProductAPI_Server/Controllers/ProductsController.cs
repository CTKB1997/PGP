using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using ProductAPI_Server.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ProductAPI_Server.Key;
using System.IO;
using System;
using System.Globalization;
using System.Reflection;

namespace ProductAPI_Server.Controllers
{
    [RoutePrefix("api/products")]
    [Authorize]
    public class ProductsController : ApiController
    {
        private ProductServiceContext db = new ProductServiceContext();
        //private string privateKeyPath = @"D:\Keys\PGPPrivateKey.asc";
        private string privateKeyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\PGPPrivateKey.asc";
        private string pwd = "P@ll@m@lli";

        // GET: api/Products
        [Route("get-all-products")]
        public IQueryable<Product> GetProducts()
        {
            return db.Products;
        }

        // GET: api/Products/5
        //[ResponseType(typeof(Product))]
        [Route("get-product/{id:int}")]
        public async Task<IHttpActionResult> GetProduct(int id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        public static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static string GenerateStringFromStream(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string text = reader.ReadToEnd();
            return text;
        }
        /// <summary>
        /// Convert an byte[] parameter to string after decode (decrypt)
        /// </summary>
        /// <param name="kObj">have byte[] parameter </param>
        /// <returns></returns>
        
        #region Convert
        private string Convert(KeyObj kObj)
        {
            Stream output = PGPDecrypt.Decrypt(kObj.kByte, privateKeyPath, pwd);
            string later = GenerateStringFromStream(output);
            return later;
        }
        #endregion

        // PUT: api/Products/5
        //[ResponseType(typeof(void))]
        [Route("update-product/{id:int}")]
        public async Task<IHttpActionResult> PutProduct(int id, JObject requestParam)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            #region Main
            var kObj = JsonConvert.DeserializeObject<KeyObj>(requestParam.ToString());
            string later = Convert(kObj);
            var item = JsonConvert.DeserializeObject<Product>(later);
            #endregion

            //Product = (Product) 
            if (id != item.ProductID)
            {
                return BadRequest();
            }

            db.Entry(item).State = EntityState.Modified;
            try
            {
                await db.SaveChangesAsync();
                return StatusCode(HttpStatusCode.OK);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // POST: api/Products
        //[ResponseType(typeof(Product))]
        [Route("create-product")]
        public async Task<IHttpActionResult> PostProduct(JObject requestParam)
        {
            //var productRequest = JsonConvert.DeserializeObject<Product>(requestParam.ToString());

            #region Main
            var kObj = JsonConvert.DeserializeObject<KeyObj>(requestParam.ToString());
            string later = Convert(kObj);
            var productRequest = JsonConvert.DeserializeObject<Product>(later);
            #endregion

            db.Products.Add(productRequest);

            await db.SaveChangesAsync();
            Product pro = await db.Products.FindAsync(productRequest.ProductID);
            return Ok(pro);
            //await db.SaveChangesAsync();

            //return CreatedAtRoute("DefaultApi", new { id = product.ProductID }, product);
        }

        // DELETE: api/Products/5
        //[ResponseType(typeof(Product))]
        [Route("delete-product/{id:int}")]
        public async Task<IHttpActionResult> DeleteProduct(int id)
        {
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            db.Products.Remove(product);
            await db.SaveChangesAsync();

            return Ok(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProductExists(int id)
        {
            return db.Products.Count(e => e.ProductID == id) > 0;
        }
    }
}