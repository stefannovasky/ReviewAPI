using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Domain.Interfaces.Repositories
{
    public interface ICommentRepository : IGenericRepository<Comment>
    {
        Task<IEnumerable<Comment>> GetAllFromReview(Guid reviewId, int page, int quantityPerPage);
        Task<int> CountFromReview(Guid reviewId);
        Task<Comment> GetByIdIncludingUser(Guid commentId);
    }
}
