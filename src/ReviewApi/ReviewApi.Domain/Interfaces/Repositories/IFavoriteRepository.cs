using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Domain.Interfaces.Repositories
{
    public interface IFavoriteRepository : IGenericRepository<Favorite>
    {
        Task<bool> AlreadyExists(Guid userId, Guid reviewId);
        Task<Favorite> GetByUserIdAndReviewId(Guid userId, Guid reviewId);
        Task<IEnumerable<Favorite>> GetAllByUserIdIncludingReview(Guid userId, int page, int quantityPerPage);
        Task<IEnumerable<Favorite>> GetAllByReviewId(Guid reviewId, int page, int quantityPerPage);
        Task<int> CountByReviewId(Guid reviewId);
        Task<int> CountByUserId(Guid userId);
    }
}
