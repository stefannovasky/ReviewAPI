using System.Threading.Tasks;
using ReviewApi.Application.Models;
using ReviewApi.Application.Models.Review;
using ReviewApi.Application.Models.User;
using ReviewApi.Domain.Dto;

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
        Task<PaginationResponseModel<UserProfileResponseModel>> GetAllFromReview(string reviewId, PaginationDTO pagination);
        Task<PaginationResponseModel<ReviewResponseModel>> GetAllFromUser(string userName, PaginationDTO pagination);
    }
}
