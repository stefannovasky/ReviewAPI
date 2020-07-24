using System.Threading.Tasks;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<bool> AlreadyExists(string email);
        Task<User> GetByEmail(string email);
        Task<User> GetByConfirmationCode(string code);
    }
}
