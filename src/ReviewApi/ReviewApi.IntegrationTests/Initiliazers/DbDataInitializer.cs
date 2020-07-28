using Microsoft.EntityFrameworkCore;
using ReviewApi.Domain.Entities;
using ReviewApi.Infra.Context;

namespace ReviewApi.IntegrationTests.Initiliazers
{
    internal class DbDataInitializer<T> where T : BaseEntity
    {
        protected MainContext _dbContext;
        protected DbSet<T> _dbSet;

        protected void Save() => _dbContext.SaveChanges();
    }
}
