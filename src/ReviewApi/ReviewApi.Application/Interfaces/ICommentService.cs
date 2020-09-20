using System.Threading.Tasks;
using ReviewApi.Application.Models;
using ReviewApi.Application.Models.Comment;

namespace ReviewApi.Application.Interfaces
{
    public interface ICommentService
    {
        Task<IdResponseModel> Create(string reviewId, string userId, CreateCommentRequestModel model);
        Task<PaginationResponseModel<CommentResponseModel>> GetAllFromReview(string reviewId, int page = 1, int quantityPerPage = 14);
        Task Delete(string commentId, string userId);
        Task<CommentResponseModel> GetById(string commentId);
        Task Update(string commentId, string userId, CreateCommentRequestModel model);
    }
}
