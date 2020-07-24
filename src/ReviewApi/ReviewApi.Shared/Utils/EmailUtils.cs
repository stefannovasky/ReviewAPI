using System.Threading.Tasks;
using ReviewApi.Domain.Dto;
using ReviewApi.Infra.Redis.Interfaces;
using ReviewApi.Shared.Interfaces;
using StackExchange.Redis;

namespace ReviewApi.Shared.Utils
{
    public class EmailUtils : IEmailUtils
    {
        private readonly IRedisConnector _redisConnector;
        private readonly IJsonUtils _jsonUtils; 

        public EmailUtils(IRedisConnector redisConnector, IJsonUtils jsonUtils)
        {
            _redisConnector = redisConnector;
            _jsonUtils = jsonUtils; 
        }

        public async Task SendEmail(string to, string subject, string body)
        {
            _redisConnector.Connect();
            ISubscriber publisher = _redisConnector.Connection.GetSubscriber();
            await publisher.PublishAsync("mail", _jsonUtils.Serialize(new EmailDTO(to, subject, body)));
        }
    }   
}
