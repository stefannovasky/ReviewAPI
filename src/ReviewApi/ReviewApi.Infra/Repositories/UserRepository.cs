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

        public async Task<User> GetByEmail(string email)
        {
            return await Query().SingleOrDefaultAsync(user => user.Email == email);
        }
    }
}
