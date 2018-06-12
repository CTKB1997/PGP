using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace ProductAPI_Client.Key
{
    public class PgpEncryptionKeys
    {
        public PgpPublicKey PublicKey { get; set; }

        public PgpEncryptionKeys(string publicKeyInfo)
        {
            if (publicKeyInfo == null)
                throw new ArgumentException("Public key file not found", "publicKeyPath");
            PublicKey = ReadPublicKey(publicKeyInfo);
        }

        #region Public Key

        private PgpPublicKey ReadPublicKey(string publicKeyInfo)
        {

            //using (Stream keyIn = File.OpenRead(publicKeyPath))
            byte[] byteArray = Encoding.UTF8.GetBytes(publicKeyInfo);
            //byte[] byteArray = Encoding.ASCII.GetBytes(contents);
            MemoryStream keyIn = new MemoryStream(byteArray);

            using (Stream inputStream = PgpUtilities.GetDecoderStream(keyIn))
            {

                PgpPublicKeyRingBundle publicKeyRingBundle = new PgpPublicKeyRingBundle(inputStream);

                PgpPublicKey foundKey = GetFirstPublicKey(publicKeyRingBundle);

                if (foundKey != null)

                    return foundKey;

            }

            throw new ArgumentException("No encryption key found in public key ring.");

        }

        private PgpPublicKey GetFirstPublicKey(PgpPublicKeyRingBundle publicKeyRingBundle)
        {

            foreach (PgpPublicKeyRing kRing in publicKeyRingBundle.GetKeyRings())
            {

                PgpPublicKey key = kRing.GetPublicKeys()

                    .Cast<PgpPublicKey>()

                    .Where(k => k.IsEncryptionKey)

                    .FirstOrDefault();

                if (key != null)

                    return key;

            }

            return null;

        }

        #endregion
        

    }
}
