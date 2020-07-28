using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ReviewApi.Infra.Context;
using ReviewApi.IntegrationTests.Initiliazers;
using Xunit;

namespace ReviewApi.IntegrationTests.CustomWebApplicationFactory
{
    [Collection("Sequential")]
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private MainContext _db;

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

                SqliteConnection connection = new SqliteConnection("Filename=TestDatabase.db");
                connection.Open();

                services.AddDbContext<MainContext>(options =>
                {
                    options.UseInMemoryDatabase(databaseName: "TestDatabase");
                });

                ServiceProvider serviceProvider = services.BuildServiceProvider();

                using (var scope = serviceProvider.CreateScope())
                {
                    IServiceProvider scopedServices = scope.ServiceProvider;
                    _db = scopedServices.GetRequiredService<MainContext>();
                    ILogger logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    _db.Database.EnsureDeleted();
                    new UsersDbDataInitializer(_db);
                    _db.Database.EnsureCreated();
                }
            });
        }
    }
}
