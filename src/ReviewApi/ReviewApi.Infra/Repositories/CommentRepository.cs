using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Infra.Context;

namespace ReviewApi.Infra.Repositories
{
    public class CommentRepository : GenericRepository<Comment>, ICommentRepository
    {
        public CommentRepository(MainContext dbContext) : base(dbContext)
        {

        }

        public async Task<int> CountFromReview(Guid reviewId)
        {
            return await Query().Where(comment => comment.ReviewId == reviewId).CountAsync();
        }

        public async Task<IEnumerable<Comment>> GetAllFromReview(Guid reviewId, int page, int quantityPerPage)
        {
            int skip = (page - 1) * quantityPerPage;
            return await Query()
                .Where(comment => comment.ReviewId == reviewId)
                .Take(quantityPerPage)
                .Skip(skip)
                .Include(comment => comment.User)
                    .ThenInclude(user => user.ProfileImage)
                .ToListAsync();
        }

        public async Task<Comment> GetByIdIncludingUser(Guid commentId)
        {
            return await Query()
                .Where(comment => comment.Id == commentId)
                .Include(comment => comment.User)
                    .ThenInclude(user => user.ProfileImage)
                .SingleOrDefaultAsync();
        }
    }
}
