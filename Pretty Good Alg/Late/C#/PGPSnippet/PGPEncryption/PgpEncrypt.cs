using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;
using PGPSnippet.Keys;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGPSnippet.PGPEncryption
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

        public void EncryptAndSign(Stream outputStream, FileInfo unencryptedFileInfo)
        {

            if (outputStream == null)

                throw new ArgumentNullException("outputStream", "outputStream is null.");

            if (unencryptedFileInfo == null)

                throw new ArgumentNullException("unencryptedFileInfo", "unencryptedFileInfo is null.");

            if (!File.Exists(unencryptedFileInfo.FullName))

                throw new ArgumentException("File to encrypt not found.");

            using (Stream encryptedOut = ChainEncryptedOut(outputStream))

            using (Stream compressedOut = ChainCompressedOut(encryptedOut))
            {

                using (Stream literalOut = ChainLiteralOut(compressedOut, unencryptedFileInfo))

                using (FileStream inputFile = unencryptedFileInfo.OpenRead())
                {

                    WriteOutputAndSign(compressedOut, literalOut, inputFile);

                }

            }

        }

        private static void WriteOutputAndSign(Stream compressedOut, Stream literalOut, FileStream inputFile)
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

        private static Stream ChainLiteralOut(Stream compressedOut, FileInfo file)
        {

            PgpLiteralDataGenerator pgpLiteralDataGenerator = new PgpLiteralDataGenerator();

            return pgpLiteralDataGenerator.Open(compressedOut, PgpLiteralData.Binary, file);

        }
    }

}
