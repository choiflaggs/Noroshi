using System.Security.Cryptography;
using System.Text;

namespace Noroshi.Server.Security
{
    public class Cryption
    {
        private static readonly string _aesIV = "O%o3Z~7Qleq-&&aU0I6!4hCc55Z#0vK-";
        private static readonly string _aesKey = ")2~8bVLU)!&fz7PG6Pzyf2%u972X)(Ti";

        private static RijndaelManaged GetRijdael()
        {
            var rijndael = new RijndaelManaged();
            rijndael.BlockSize = 256;
            rijndael.KeySize = 256;
            rijndael.IV = Encoding.UTF8.GetBytes(_aesIV);
            rijndael.Key = Encoding.UTF8.GetBytes(_aesKey);
            rijndael.Mode = CipherMode.CBC;
            rijndael.Padding = PaddingMode.PKCS7;
            return rijndael;
        }

        public static byte[] Encrypt(byte[] data)
        {
            var rijndel = GetRijdael();
            using (var encrypt = rijndel.CreateEncryptor())
            {
                return encrypt.TransformFinalBlock(data, 0, data.Length);
            }
        }

        public static byte[] Dencrypt(byte[] data)
        {
            var rijndel = GetRijdael();
            using (var encrypt = rijndel.CreateDecryptor())
            {
                return encrypt.TransformFinalBlock(data, 0, data.Length);
            }
        }

    }
}