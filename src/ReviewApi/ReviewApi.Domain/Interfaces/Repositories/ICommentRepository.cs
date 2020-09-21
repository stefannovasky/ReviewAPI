using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewApi.Domain.Dto;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Domain.Interfaces.Repositories
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetAllFromReview(Guid reviewId, PaginationDTO pagination);
        Task<int> CountFromReview(Guid reviewId);
        Task<Comment> GetByIdIncludingUser(Guid commentId);
    }
}
