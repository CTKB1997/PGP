using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ProductAPI_Server.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Principal;
using System.Threading;
using System.Web;
using System.IO;
using System.Net.Http.Headers;
using System;
using System.Reflection;

namespace ProductAPI_Server.Controllers
{
    [RoutePrefix("api/acc")]
    public class TKsController : ApiController
    {
        private ProductServiceContext db = new ProductServiceContext();
        private string publicKeyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\PGPPublicKey.asc";

        // GET: api/TKs
        public IQueryable<TK> GetTKs()
        {
            return db.TKs;
        }

        // GET: api/TKs/5
        [ResponseType(typeof(TK))]
        [Authorize]
        public async Task<IHttpActionResult> GetTK(string id)
        {
            TK tK = await db.TKs.FindAsync(id);
            if (tK == null)
            {
                return NotFound();
            }

            return Ok(tK);
        }

        // Login: api/account
        /*
        [HttpGet]
        [Route("{id}/{password}")]
        public async Task<IHttpActionResult> CheckLogin(string id, string password)
        {
            TK tK = await db.TKs.FindAsync(id);
            if (tK == null || tK.Password != password)
            {
                return NotFound();
            }

            return Ok(tK);
        }
        */
        private void SetPrincipal(IPrincipal principal)
        {
            Thread.CurrentPrincipal = principal;
            if (HttpContext.Current != null)
            {
                HttpContext.Current.User = principal;
            }
        }

        [Route("login")]
        public async Task<IHttpActionResult> CheckLogin(JObject requestParam)
        {
            var loginAccount = JsonConvert.DeserializeObject<TK>(requestParam.ToString());
            TK tK = await db.TKs.FindAsync(loginAccount.Id);
            if (tK == null || tK.Password != loginAccount.Password)
            {
                return NotFound();
            }
            return Ok(tK);
        }

        [Route("get-public-key")]
        public HttpResponseMessage GetPublicKey()
        {
            //PgpEncryptionKeys encryptionKeys = new PgpEncryptionKeys(@"D:\Keys\PGPPublicKey.asc");
            //string text = System.IO.File.ReadAllText(@"D:\Keys\PGPPublicKey.asc");
            string text = System.IO.File.ReadAllText(publicKeyPath);

            return new HttpResponseMessage
            {
                Content = new JsonContent(new
                {
                    success = true,
                    file = text
                }),
                StatusCode = HttpStatusCode.OK
            };
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TKExists(string id)
        {
            return db.TKs.Count(e => e.Id == id) > 0;
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
            return _Stream.CopyToAsync(stream);
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _Stream.Length;
            return true;
        }
    }
}

/*
 * 
 * 
        // PUT: api/TKs/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTK(string id, TK tK)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tK.Id)
            {
                return BadRequest();
            }

            db.Entry(tK).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TKExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/TKs
        [ResponseType(typeof(TK))]
        public async Task<IHttpActionResult> PostTK(TK tK)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TKs.Add(tK);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (TKExists(tK.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = tK.Id }, tK);
        }

        // DELETE: api/TKs/5
        [ResponseType(typeof(TK))]
        public async Task<IHttpActionResult> DeleteTK(string id)
        {
            TK tK = await db.TKs.FindAsync(id);
            if (tK == null)
            {
                return NotFound();
            }

            db.TKs.Remove(tK);
            await db.SaveChangesAsync();

            return Ok(tK);
        }

    */
