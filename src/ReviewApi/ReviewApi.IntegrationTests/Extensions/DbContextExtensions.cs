using Microsoft.EntityFrameworkCore;

namespace ReviewApi.IntegrationTests.Extensions
{
    internal static class DbContextExtensions
    {
        public static void ResetDatabase(this DbContext context)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }
    }
}
