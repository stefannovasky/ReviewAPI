using ReviewApi.Infra.Redis.Interfaces;
using StackExchange.Redis;

namespace ReviewApi.Infra.Redis
{
    public class RedisConnector : IRedisConnector
    {
        private readonly string _connectionString;
        public ConnectionMultiplexer Connection { get; private set; }

        public RedisConnector(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Connect()
        {
            Connection = ConnectionMultiplexer.Connect(new ConfigurationOptions()
            {
                ConnectTimeout = 15000, 
                EndPoints = { _connectionString }
            }); 
        }
    }
}
