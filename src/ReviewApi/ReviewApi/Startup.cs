using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using ReviewApi.Application.Converter;
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

            ConfigureRedisCache(services);
            ConfigureJwtToken(services);
            ConfigureSwaggerDoc(services);
            SetDependencyInjection(services);
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

            app.UseSwagger();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "ReviewAPI"));

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

        private void ConfigureJwtToken(IServiceCollection services)
        {
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
        }

        private void ConfigureRedisCache(IServiceCollection services)
        {
            services.AddDistributedRedisCache(options =>
            {
                options.Configuration =
                    Configuration.GetConnectionString("RedisConnection");
                options.InstanceName = "ReviewApi";
            });
        }

        private void ConfigureSwaggerDoc(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "ReviewAPI",
                    Version = "v1",
                    Description = "REST API where users can analyze anything.",
                });
            });
        }

        private void SetDependencyInjection(IServiceCollection services)
        {
            services.AddTransient<ICommentRepository, CommentRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IProfileImageRepository, ProfileImageRepository>();
            services.AddTransient<IFavoriteRepository, FavoriteRepository>();
            services.AddTransient<IReviewRepository, ReviewRepository>();

            services.AddTransient<IRedisConnector>(service => new RedisConnector(Configuration.GetConnectionString("RedisConnection")));
            services.AddTransient<ICacheDatabase>(services => new CacheDatabase(Configuration.GetConnectionString("RedisConnection")));

            services.AddTransient<IRandomCodeUtils, RandomCodeUtils>();
            services.AddTransient<IHashUtils, HashUtils>();
            services.AddTransient<IEmailUtils, EmailUtils>();
            services.AddTransient<IJsonUtils, JsonUtils>();
            services.AddTransient<IJwtTokenUtils>(service => new JwtTokenUtils(Configuration.GetValue<string>("Secret")));
            services.AddTransient<IFileUploadUtils>(service => new FileUploadUtils(_webApplicationRootPath, _webApplicationUrl));
            services.AddTransient<ILogUtils, LogUtils>();
            services.AddTransient<IConverter, Converter>();


            services.AddTransient<ICommentService>(service => new CommentService(
                service.GetRequiredService<ICommentRepository>(), service.GetRequiredService<IReviewRepository>(), service.GetRequiredService<ICacheDatabase>(), service.GetRequiredService<IJsonUtils>(), service.GetRequiredService<IConverter>(), _webApplicationUrl
            ));
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IFavoriteService>(service => new FavoriteService(
                service.GetRequiredService<IFavoriteRepository>(),
                service.GetRequiredService<IReviewRepository>(),
                service.GetRequiredService<IUserRepository>(),
                service.GetRequiredService<IConverter>(),
                _webApplicationUrl
            ));
            services.AddTransient<IReviewService>(service => new ReviewService(
                service.GetRequiredService<IReviewRepository>(), service.GetRequiredService<IFileUploadUtils>(), service.GetRequiredService<ICacheDatabase>(), service.GetRequiredService<IJsonUtils>(), service.GetRequiredService<IConverter>(), _webApplicationUrl)
            );
        }
    }
}
