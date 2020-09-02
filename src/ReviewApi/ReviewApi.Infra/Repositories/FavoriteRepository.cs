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
    public class FavoriteRepository : GenericRepository<Favorite>, IFavoriteRepository
    {
        public FavoriteRepository(MainContext dbContext) : base(dbContext)
        {

        }

        public async Task<bool> AlreadyExists(Guid userId, Guid reviewId)
        {
            return await Query().AnyAsync(favorite => favorite.UserId == userId && favorite.ReviewId == reviewId);
        }

        public async Task<int> CountByReviewId(Guid reviewId)
        {
            return await Query().CountAsync(favorite => favorite.ReviewId == reviewId);
        }

        public async Task<IEnumerable<Favorite>> GetByReviewId(Guid reviewId, int page, int quantityPerPage)
        {
            int skip = (page - 1) * quantityPerPage;
            return await Query()
                .Where(favorite => favorite.ReviewId == reviewId)
                .Skip(skip)
                .Take(quantityPerPage)
                .Include(favorite => favorite.User)
                    .ThenInclude(user => user.ProfileImage) 
                .ToListAsync();
        }

        public async Task<Favorite> GetByUserIdAndReviewId(Guid userId, Guid reviewId)
        {
            return await Query().SingleOrDefaultAsync(favorite => favorite.UserId == userId && favorite.ReviewId == reviewId);
        }
    }
}
