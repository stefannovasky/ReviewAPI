using System.Threading.Tasks;
using ReviewApi.Application.Models;
using ReviewApi.Application.Models.Review;

namespace ReviewApi.Application.Interfaces
{
    public interface IReviewService
    {
        Task<IdResponseModel> Create(string userId, CreateOrUpdateReviewRequestModel model);
    }
}
