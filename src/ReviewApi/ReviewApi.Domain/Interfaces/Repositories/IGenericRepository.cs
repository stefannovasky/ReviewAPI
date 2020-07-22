using System;
using System.Threading.Tasks;
using ReviewApi.Domain.Entities;

namespace ReviewApi.Domain.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {
        Task<T> GetById(Guid id);
        Task Create(T entity);
        void Update(T entity);
        Task<bool> AlreadyExists(Guid id);
        Task Save();
    }
}
