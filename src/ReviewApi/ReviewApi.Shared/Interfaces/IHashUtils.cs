using System.Threading.Tasks;

namespace ReviewApi.Shared.Interfaces
{
    public interface IHashUtils
    {
        string GenerateHash(string plaintext);
        bool CompareHash(string plaintext, string hash);
    }
}
