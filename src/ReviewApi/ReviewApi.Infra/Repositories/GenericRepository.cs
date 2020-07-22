using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Infra.Context;

namespace ReviewApi.Infra.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly MainContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(MainContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<bool> AlreadyExists(Guid id)
        {
            try
            {
                return await Query().AnyAsync(entity => !entity.Deleted && entity.Id == id);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public async Task Create(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public async Task<T> GetById(Guid id)
        {
            try
            {
                return await Query().SingleOrDefaultAsync(entity => !entity.Deleted && entity.Id == id);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public async Task Save()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public void Update(T entity)
        {
            try
            {
                _dbSet.Update(entity);
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        protected IQueryable<T> Query() => _dbSet.AsNoTracking();
    }
}
