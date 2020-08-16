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
            return await _dbSet.AnyAsync(user => user.Email == email);
        }

        public async Task<User> GetByConfirmationCode(string code)
        {
            return await Query().SingleOrDefaultAsync(user => user.ConfirmationCode == code);
        }

        public async Task<User> GetByEmail(string email)
        {
            return await Query().SingleOrDefaultAsync(user => user.Email == email);
        }

        public async Task<User> GetByIdIncludingImage(Guid id)
        {
            return await Query().Include(user => user.ProfileImage).SingleOrDefaultAsync(user => user.Id == id);
        }

        public async Task<User> GetByName(string name)
        {
            return await Query().SingleOrDefaultAsync(user => user.Name == name);
        }

        public async Task<User> GetByNameIncludingImage(string name)
        {
            return await Query().Include(u => u.ProfileImage).SingleOrDefaultAsync(user => user.Name == name);
        }

        public async Task<User> GetByNameIncludingReviews(string name)
        {
            return await Query().Include(u => u.Reviews).ThenInclude(r => r.Image).SingleOrDefaultAsync(u => u.Name == name);
        }

        public async Task<User> GetByResetPasswordCode(string code)
        {
            return await Query().SingleOrDefaultAsync(user => user.ResetPasswordCode == code);
        }
    }
}
