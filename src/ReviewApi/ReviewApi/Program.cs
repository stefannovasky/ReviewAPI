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
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureServices(services =>
                {
                    IConfigurationRoot configs = new ConfigurationBuilder().AddJsonFile("appsettings.json", false).Build();

                    services.AddHostedService<EmailWorker>(worker => new EmailWorker(
                       worker.GetRequiredService<IRedisConnector>(), worker.GetRequiredService<IJsonUtils>(), configs.GetSection("EmailConfiguration").Get<EmailConfiguration>()
                    ));
                });
    }
}
