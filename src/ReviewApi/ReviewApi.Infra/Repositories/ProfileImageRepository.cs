using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Infra.Context;

namespace ReviewApi.Infra.Repositories
{
    public class ProfileImageRepository : GenericRepository<ProfileImage>, IProfileImageRepository
    {
        public ProfileImageRepository(MainContext dbContext) : base(dbContext)
        {
        }
    }
}
