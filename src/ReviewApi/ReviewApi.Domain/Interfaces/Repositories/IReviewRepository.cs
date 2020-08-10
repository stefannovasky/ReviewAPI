using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Domain.Interfaces.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetAll(int page = 1, int quantityPerPage = 14);
    }
}
