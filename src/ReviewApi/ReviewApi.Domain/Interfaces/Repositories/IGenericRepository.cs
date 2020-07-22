using System.Threading.Tasks;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Domain.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetById(int id);
        Task Create(T entity);
        void Update(T entity);
        Task Save();
    }
}
