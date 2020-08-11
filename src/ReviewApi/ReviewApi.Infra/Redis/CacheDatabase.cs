using System.Threading.Tasks;
using ReviewApi.Infra.Redis.Interfaces;
using StackExchange.Redis;

namespace ReviewApi.Infra.Redis
{
    public class CacheDatabase : RedisConnector, ICacheDatabase
    {
        private readonly IDatabase _cacheDatabase;
        public CacheDatabase(string connectionString) : base(connectionString)
        {
            Connect();
            _cacheDatabase = Connection.GetDatabase();
        }

        public async Task Remove(string key)
        {
            await _cacheDatabase.KeyDeleteAsync(key);
        }

        public async Task Set(string key, string jsonValue)
        {
            await _cacheDatabase.StringSetAsync(key, jsonValue);
        }

        public async Task<string> Get(string key)
        {
            return await _cacheDatabase.StringGetAsync(key);
        }
    }
}
