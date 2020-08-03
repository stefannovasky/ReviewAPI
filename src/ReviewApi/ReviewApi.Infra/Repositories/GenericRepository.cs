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

        public async Task<bool> AlreadyExists(Guid id)
        {
            try
            {
                return await Query().AnyAsync(entity => entity.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Create(T entity)
        {
            try
            {
                await _dbSet.AddAsync(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Delete(T entity)
        {
            try
            {
                _dbSet.Attach(entity);
                _dbSet.Remove(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<T> GetById(Guid id)
        {
            try
            {
                return await Query().SingleOrDefaultAsync(entity => entity.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task Save()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void Update(T entity)
        {
            try
            {
                _dbContext.Update(entity);
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected IQueryable<T> Query() => _dbSet.AsNoTracking();
    }
}
