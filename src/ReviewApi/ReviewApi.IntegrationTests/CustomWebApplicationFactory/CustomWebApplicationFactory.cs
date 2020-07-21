using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReviewApi.Infra.Context;

namespace ReviewApi.IntegrationTests.CustomWebApplicationFactory
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private MainContext _database;

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                ServiceDescriptor serviceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType ==
                        typeof(DbContextOptions<MainContext>));

                if (serviceDescriptor != null)
                {
                    services.Remove(serviceDescriptor);
                }

                SqliteConnection connection = new SqliteConnection("Filename=:memory:");
                connection.Open();

                services.AddDbContext<MainContext>(options =>
                {
                    options.UseSqlite(connection);
                });

                ServiceProvider serviceProvider = services.BuildServiceProvider();

                using (var scope = serviceProvider.CreateScope())
                {
                    IServiceProvider scopedServices = scope.ServiceProvider;
                    _database = scopedServices.GetRequiredService<MainContext>();
                    ILogger logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    _database.Database.EnsureDeleted();
                    _database.Database.EnsureCreated();
                }
            });
        }
    }
}
