using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Infra.Context;

namespace ReviewApi.Infra.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity, new()
    {
        protected readonly MainContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(MainContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public virtual async Task<bool> AlreadyExists(Guid id)
        {
            return await Query().AnyAsync(entity => entity.Id == id);
        }

        public async Task<int> Count()
        {
            return await Query().CountAsync();
        }

        public virtual async Task Create(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbSet.Attach(entity);
            _dbSet.Remove(entity);
        }

        public virtual async Task<T> GetById(Guid id)
        {
            return await Query().SingleOrDefaultAsync(entity => entity.Id == id);
        }

        public virtual async Task Save()
        {
            await _dbContext.SaveChangesAsync();
        }

        public virtual void Update(T entity)
        {
            _dbContext.Update(entity);
        }

        protected IQueryable<T> Query() => _dbSet.AsNoTracking();
    }
}
