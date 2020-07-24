using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using ReviewApi.Domain.Dto;
using ReviewApi.Infra.Redis.Interfaces;
using ReviewApi.Shared.Interfaces;
using ReviewApi.Workers.Configurations;
using StackExchange.Redis;

namespace ReviewApi.Workers
{
    public sealed class EmailWorker : IHostedService
    {
        private readonly IRedisConnector _redisConnector;
        private readonly IJsonUtils _jsonUtils;
        private readonly SmtpClient _client;
        private readonly EmailConfiguration _emailConfiguration;
        private static Semaphore _mailSendSemaphore;

        public EmailWorker(IRedisConnector redisConnector, IJsonUtils jsonUtils, EmailConfiguration emailConfiguration)
        {
            _redisConnector = redisConnector;
            _jsonUtils = jsonUtils;
            _emailConfiguration = emailConfiguration;
            _mailSendSemaphore = new Semaphore(1, 1);

            _client = new SmtpClient(_emailConfiguration.SmtpServer, Convert.ToInt32(_emailConfiguration.Port))
            {
                Credentials = new NetworkCredential(_emailConfiguration.Username, _emailConfiguration.Password),
                EnableSsl = true
            };
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _redisConnector.Connect();
            ISubscriber subscriber = _redisConnector.Connection.GetSubscriber();

            subscriber.Subscribe("mail", (channel, message) =>
            {
                SendEmail(message);
            }, CommandFlags.None);
            return Task.CompletedTask;
        }

        private void SendEmail(string emailMessageJson)
        {
            EmailDTO deserializedObject = _jsonUtils.Deserialize<EmailDTO>(emailMessageJson);
            _mailSendSemaphore.WaitOne();
            try
            {
                _client.Send(_emailConfiguration.From, deserializedObject.To, deserializedObject.Subject, deserializedObject.Body);
            }
            finally
            {
                _mailSendSemaphore.Release();
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }
}
