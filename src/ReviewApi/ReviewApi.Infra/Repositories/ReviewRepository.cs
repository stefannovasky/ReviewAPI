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
    }
}
