using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReviewApi.Domain.Dto;
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

        public async Task<IEnumerable<Review>> GetAll(PaginationDTO pagination)
        {
            int skip = (pagination.Page - 1) * pagination.QuantityPerPage;
            return await Query()
                .Skip(skip)
                .Take(pagination.QuantityPerPage)
                .Include(r => r.Image)
                .Include(r => r.Creator)
                .ToListAsync();
        }

        public override async Task<Review> GetById(Guid id)
        {
            return await Query()
                .Include(r => r.Image)
                .Include(r => r.Creator)
                .SingleOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Review>> Search(string text)
        {
            string textInLowerCase = text.ToLower();
            return await Query()
                .Include(r => r.Image)
                .Include(r => r.Creator)
                .Where(r => r.Title.Contains(textInLowerCase))
                .ToListAsync();
        }
    }
}
