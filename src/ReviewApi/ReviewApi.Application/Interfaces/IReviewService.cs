using System.Collections.Generic;
using System.Threading.Tasks;
using ReviewApi.Application.Models;
using ReviewApi.Application.Models.Review;
using ReviewApi.Domain.Dto;

namespace ReviewApi.Application.Interfaces
{
    public interface IReviewService
    {
        Task<IdResponseModel> Create(string userId, CreateOrUpdateReviewRequestModel model);
        Task<PaginationResponseModel<ReviewResponseModel>> GetAll(PaginationDTO pagination);
        Task Delete(string userId, string reviewId);
        Task Update(string userId, string reviewId, CreateOrUpdateReviewRequestModel model);
        Task<ReviewResponseModel> GetById(string reviewId);
        Task<IEnumerable<ReviewResponseModel>> Search(string text);
    }
}
