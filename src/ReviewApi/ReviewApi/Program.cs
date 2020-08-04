using System.IO;
using System.Reflection;
using log4net.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReviewApi.Infra.Redis.Interfaces;
using ReviewApi.Shared.Interfaces;
using ReviewApi.Workers;
using ReviewApi.Workers.Configurations;

namespace ReviewApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ILoggerRepository log4netRepository = log4net.LogManager.GetRepository(Assembly.GetEntryAssembly());
            log4net.Config.XmlConfigurator.Configure(log4netRepository, new FileInfo("log4net.config"));

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices(services =>
                {
                    IConfigurationRoot configs = new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();

                    services.AddHostedService<EmailWorker>(worker => new EmailWorker(
                       worker.GetRequiredService<IRedisConnector>(), worker.GetRequiredService<IJsonUtils>(), configs.GetSection("EmailConfiguration").Get<EmailConfiguration>()
                    ));
                });
    }
}
