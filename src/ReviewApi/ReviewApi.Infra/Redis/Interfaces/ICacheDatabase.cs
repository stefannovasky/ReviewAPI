using System.Threading.Tasks;

namespace ReviewApi.Infra.Redis.Interfaces
{
    public interface ICacheDatabase : IRedisConnector
    {
        Task Set(string key, string jsonValue);
        Task<string> Get(string key);
        Task Remove(string key);
    }
}
