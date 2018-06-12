using PGPSnippet.Keys;
using PGPSnippet.PGPDecryption;
using PGPSnippet.PGPEncryption;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PGPSnippet
{
    class Program
    {

        public async void KeyGeneration()
        {
            #region PublicKey and Private Key Generation

            PGPSnippet.KeyGeneration.KeysForPGPEncryptionDecryption.GenerateKey("maruthi", "P@ll@m@lli", @"D:\Keys\");
            Console.WriteLine("Keys Generated Successfully");

            #endregion
        }

        public async void Encryption()
        {
            #region PGP Encryption

            PgpEncryptionKeys encryptionKeys = new PgpEncryptionKeys(@"D:\Keys\PGPPublicKey.asc");
            PgpEncrypt encrypter = new PgpEncrypt(encryptionKeys);
            using (Stream outputStream = File.Create("D:\\Keys\\EncryptData.txt"))
            {
                encrypter.EncryptAndSign(outputStream, new FileInfo(@"D:\Keys\PlainText.txt"));
            }
            Console.WriteLine("Encryption Done !");

            #endregion
        }

        public async void Decryption()
        {

            #region PGP Decryption

            PGPDecrypt.Decrypt("D:\\Keys\\EncryptData.txt", @"D:\Keys\PGPPrivateKey.asc", "P@ll@m@lli" , "D:\\Keys\\OriginalText.txt");

            Console.WriteLine("Decryption Done");

            #endregion
        }


        static void Main(string[] args)
        {

            Program objPGP = new Program();
            try
            {

                objPGP.KeyGeneration();
                #region Register Key
                /* Input: Account + Password + Dia diem save key
                 * Output: PGPPrivateKey.asc => save private key
                 *         PGPPublicKey.asc => save public key 
                 */
                #endregion
                //objPGP.Encryption();
                #region Encrypt Data
                /* Input: public key
                 *        vi tri save file sau khi ma hoa + vi tri file can ma hoa  
                 * Output: EncryptData.txt => file sau khi ma hoa
                 */
                #endregion
                //objPGP.Decryption();
                #region Decrypt Data
                /* Input: vi tri file sau khi ma hoa + private key + mat khau + vi tri luu file sau khi dich nguoc
                 * Output: OriginalText.txt => file sau khi dich nguoc
                 */
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("Some thing went wrong");
                Console.Read();
            }
        }
    }
}
#region References
/*https://code.msdn.microsoft.com/windowsdesktop/Pretty-Good-Privacy-using-4f473c67
 * 
 */
#endregion