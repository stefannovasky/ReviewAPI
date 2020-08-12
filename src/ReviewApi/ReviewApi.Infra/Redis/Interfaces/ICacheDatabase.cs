using System;
using System.Threading.Tasks;

namespace ReviewApi.Infra.Redis.Interfaces
{
    public interface ICacheDatabase : IRedisConnector
    {
        Task Set(string key, string jsonValue, int expirationTimeInSeconds = 300);
        Task<string> Get(string key);
        Task Remove(string key);
    }
}
