using System.Threading.Tasks;
using ReviewApi.Application.Models;
using ReviewApi.Application.Models.User;

namespace ReviewApi.Application.Interfaces
{
    public interface IFavoriteService
    {
        /// <summary>
        ///     Create favorite if favorite not exists.
        ///     Delete favorite if already exists. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="reviewId"></param>
        /// <returns></returns>
        Task CreateOrDelete(string userId, string reviewId);
        Task<PaginationResponseModel<UserProfileResponseModel>> GetAllFromReview(string reviewId, int page = 1, int quantityPerPage = 14);
    }
}
