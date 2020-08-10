using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Infra.Context;

namespace ReviewApi.Infra.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(MainContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> AlreadyExists(string email)
        {
            try
            {
                return await _dbSet.AnyAsync(user => user.Email == email);     
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> GetByConfirmationCode(string code)
        {
            try
            {
                return await Query().SingleOrDefaultAsync(user => user.ConfirmationCode == code);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> GetByEmail(string email)
        {
            try
            {
                return await Query().SingleOrDefaultAsync(user => user.Email == email);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> GetByIdIncludingImage(Guid id)
        {
            try
            {
                return await Query().Include(user => user.ProfileImage).SingleOrDefaultAsync(user => user.Id == id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> GetByName(string name)
        {
            try
            {
                return await Query().SingleOrDefaultAsync(user => user.Name == name);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<User> GetByNameIncludingImage(string name)
        {
            return await Query().Include(u => u.ProfileImage).SingleOrDefaultAsync(user => user.Name == name);
        }

        public async Task<User> GetByResetPasswordCode(string code)
        {
            try
            {
                return await Query().SingleOrDefaultAsync(user => user.ResetPasswordCode == code);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
