using System;
using System.Threading.Tasks;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Domain.Interfaces.Repositories
{
    public interface IFavoriteRepository : IGenericRepository<Favorite>
    {
        Task<bool> AlreadyExists(Guid userId, Guid reviewId);
        Task<Favorite> GetByUserIdAndReviewId(Guid userId, Guid reviewId);
    }
}
