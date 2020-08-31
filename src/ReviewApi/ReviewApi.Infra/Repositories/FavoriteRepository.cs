using System;
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
    }
}
