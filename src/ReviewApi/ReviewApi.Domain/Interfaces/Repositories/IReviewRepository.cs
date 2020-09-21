using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewApi.Domain.Dto;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Domain.Interfaces.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetAll(PaginationDTO pagination);
        Task<IEnumerable<Review>> Search(string text);
    }
}
