using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ReviewApi.Shared.Interfaces;

namespace ReviewApi.Shared.Utils
{
    public class HashUtils : IHashUtils
    {
        public bool CompareHash(string plaintext, string hash)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                plaintext = GenerateHash(plaintext);
                return plaintext == hash;
            }
        }

        public string GenerateHash(string plaintext)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(plaintext));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
