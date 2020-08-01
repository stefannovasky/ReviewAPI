using StackExchange.Redis;

namespace ReviewApi.Infra.Redis.Interfaces
{
    public interface IRedisConnector
    {
        void Disconnect();
        void Connect();
        ConnectionMultiplexer Connection { get; }
    }
}
