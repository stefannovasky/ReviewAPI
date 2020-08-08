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
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        public ReviewRepository(MainContext dbContext) : base(dbContext)
        {

        }

        public async Task<IEnumerable<Review>> GetAll(int page = 1, int quantityPerPage = 14)
        {
            int skip = (page - 1) * quantityPerPage;
            return await Query()
                .Skip(skip)
                .Take(quantityPerPage)
                .Include(r => r.Image)
                .Include(r => r.Creator)
                .ToListAsync();
        }
    }
}
