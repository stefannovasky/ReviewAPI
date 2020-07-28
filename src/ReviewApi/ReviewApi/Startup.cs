using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Services;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Infra.Context;
using ReviewApi.Infra.Redis;
using ReviewApi.Infra.Redis.Interfaces;
using ReviewApi.Infra.Repositories;
using ReviewApi.Shared.Interfaces;
using ReviewApi.Shared.Utils;

namespace ReviewApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            AddDbContext(services);
            services.AddControllers();
            services.AddCors();

            byte[] key = Encoding.UTF8.GetBytes(Configuration["Secret"]);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();

            services.AddTransient<IRedisConnector>(service => new RedisConnector(Configuration.GetConnectionString("RedisConnection")));

            services.AddTransient<IConfirmationCodeUtils, ConfirmationCodeUtils>();
            services.AddTransient<IHashUtils, HashUtils>();
            services.AddTransient<IEmailUtils, EmailUtils>();
            services.AddTransient<IJsonUtils, JsonUtils>();
            services.AddTransient<IJwtTokenUtils>(service => new JwtTokenUtils(Configuration.GetValue<string>("Secret")));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            using (IServiceScope serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                MainContext context = serviceScope
                    .ServiceProvider
                    .GetRequiredService<MainContext>();

                if (context.Database.ProviderName != "Microsoft.EntityFrameworkCore.Sqlite")
                {
                    context.Database.Migrate();
                }
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseAuthentication();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void AddDbContext(IServiceCollection services)
        {
            services.AddDbContext<MainContext>(options => options.UseSqlServer(Configuration.GetConnectionString("SqlServerConnection")));
        }
    }
}
