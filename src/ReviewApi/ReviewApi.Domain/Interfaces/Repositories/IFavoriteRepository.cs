using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewApi.Domain.Dto;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Domain.Interfaces.Repositories
{
    public interface IFavoriteRepository : IGenericRepository<Favorite>
    {
        Task<bool> AlreadyExists(Guid userId, Guid reviewId);
        Task<Favorite> GetByUserIdAndReviewId(Guid userId, Guid reviewId);
        Task<IEnumerable<Favorite>> GetAllByUserIdIncludingReview(Guid userId, PaginationDTO pagination);
        Task<IEnumerable<Favorite>> GetAllByReviewId(Guid reviewId, PaginationDTO pagination);
        Task<int> CountByReviewId(Guid reviewId);
        Task<int> CountByUserId(Guid userId);
    }
}
