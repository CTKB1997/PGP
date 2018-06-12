using Microsoft.Owin;
using Owin;
using System.IO;
using System.Reflection;

[assembly: OwinStartup(typeof(ProductAPI_Server.Startup))]

namespace ProductAPI_Server
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            #region PublicKey and Private Key Generation

            ProductAPI_Server.Key.KeysForPGPEncryptionDecryption.GenerateKey("server", "P@ll@m@lli", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\");

            #endregion
            ConfigureAuth(app);
        }
    }
}
