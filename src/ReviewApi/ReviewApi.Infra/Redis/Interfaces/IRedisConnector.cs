using StackExchange.Redis;

namespace ReviewApi.Infra.Redis.Interfaces
{
    public interface IRedisConnector
    {
        void Connect();
        ConnectionMultiplexer Connection { get; }
    }
}
