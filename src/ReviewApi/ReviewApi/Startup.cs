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
        private readonly string _webApplicationRootPath;
        private readonly string _webApplicationUrl;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _webApplicationRootPath = env.WebRootPath;
            _webApplicationUrl = Configuration.GetValue<string>("WebApplicationUrl");
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

            services.AddTransient<IImageRepository, ImageRepository>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserService, UserService>();

            services.AddTransient<IRedisConnector>(service => new RedisConnector(Configuration.GetConnectionString("RedisConnection")));

            services.AddTransient<IRandomCodeUtils, RandomCodeUtils>();
            services.AddTransient<IHashUtils, HashUtils>();
            services.AddTransient<IEmailUtils, EmailUtils>();
            services.AddTransient<IJsonUtils, JsonUtils>();
            services.AddTransient<IJwtTokenUtils>(service => new JwtTokenUtils(Configuration.GetValue<string>("Secret")));
            services.AddTransient<IFileUploadUtils>(service => new FileUploadUtils(_webApplicationRootPath, _webApplicationUrl));
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

                if (context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
                {
                    context.Database.Migrate();
                }
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

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
