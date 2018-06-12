using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using System;
using System.IO;

namespace ProductAPI_Client.Key
{
    public class PgpEncrypt
    {

        private PgpEncryptionKeys m_encryptionKeys;

        private const int BufferSize = 0x10000; // should always be power of 2 

        /// <summary>

        /// Instantiate a new PgpEncrypt class with initialized PgpEncryptionKeys.

        /// </summary>

        /// <param name="encryptionKeys"></param>

        /// <exception cref="ArgumentNullException">encryptionKeys is null</exception>

        public PgpEncrypt(PgpEncryptionKeys encryptionKeys)
        {

            if (encryptionKeys == null)

                throw new ArgumentNullException("encryptionKeys", "encryptionKeys is null.");

            m_encryptionKeys = encryptionKeys;

        }

        /// <summary>

        /// Encrypt and sign the file pointed to by unencryptedFileInfo and

        /// write the encrypted content to outputStream.

        /// </summary>

        /// <param name="outputStream">The stream that will contain the

        /// encrypted data when this method returns.</param>

        /// <param name="fileName">FileInfo of the file to encrypt</param>

        public void EncryptAndSign(Stream outputStream, string unencryptedFileInfo)
        {
            using (Stream encryptedOut = ChainEncryptedOut(outputStream))

            using (Stream compressedOut = ChainCompressedOut(encryptedOut))
            {
                using (Stream literalOut = ChainLiteralOut(compressedOut, unencryptedFileInfo))
                {
                    WriteOutputAndSign(compressedOut, literalOut, GenerateStreamFromString(unencryptedFileInfo));
                }
            }
        }

        private static void WriteOutputAndSign(Stream compressedOut, Stream literalOut, Stream inputFile)
        {
            int length = 0;
            byte[] buf = new byte[BufferSize];
            while ((length = inputFile.Read(buf, 0, buf.Length)) > 0)
            {
                literalOut.Write(buf, 0, length);
            }
        }

        private Stream ChainEncryptedOut(Stream outputStream)
        {

            PgpEncryptedDataGenerator encryptedDataGenerator;

            encryptedDataGenerator =

                new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.TripleDes,
                                              new SecureRandom());

            encryptedDataGenerator.AddMethod(m_encryptionKeys.PublicKey);

            return encryptedDataGenerator.Open(outputStream, new byte[BufferSize]);

        }

        private static Stream ChainCompressedOut(Stream encryptedOut)
        {

            PgpCompressedDataGenerator compressedDataGenerator =

                new PgpCompressedDataGenerator(CompressionAlgorithmTag.Zip);

            return compressedDataGenerator.Open(encryptedOut);

        }

        private static Stream ChainLiteralOut(Stream compressedOut, string file)
        {

            PgpLiteralDataGenerator pgpLiteralDataGenerator = new PgpLiteralDataGenerator();

            return pgpLiteralDataGenerator.Open(compressedOut, PgpLiteralData.Binary, file, file.Length, DateTime.Now);

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

}
