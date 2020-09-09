using System;
using System.Threading.Tasks;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        Task<User> GetByIdIncludingImage(Guid id);
        Task<bool> AlreadyExists(string name);
        Task<User> GetByEmail(string email);
        Task<User> GetByName(string name);
        Task<User> GetByNameIncludingImage(string name);
        Task<User> GetByNameIncludingReviews(string name);
        Task<User> GetByConfirmationCode(string code);
        Task<User> GetByResetPasswordCode(string code);
    }
}
