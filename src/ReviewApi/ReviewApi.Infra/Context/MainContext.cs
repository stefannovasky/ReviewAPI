using Microsoft.EntityFrameworkCore;
using ReviewApi.Infra.Mappings;

namespace ReviewApi.Infra.Context
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions options) : base(options)
        {

        }

        public MainContext()
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MainContext).Assembly);

            modelBuilder.ApplyConfiguration(new UserMapConfig());
            modelBuilder.ApplyConfiguration(new ProfileImageMapConfig());
            modelBuilder.ApplyConfiguration(new ReviewMapConfig());
            modelBuilder.ApplyConfiguration(new ReviewImageMapConfig());

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder
                    .UseLazyLoadingProxies()
                    .UseSqlServer("SqlServerConnection");
            }
        }
    }
}
