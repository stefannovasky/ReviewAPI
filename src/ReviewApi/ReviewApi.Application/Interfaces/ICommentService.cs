using System.Threading.Tasks;
using ReviewApi.Application.Models;
using ReviewApi.Application.Models.Comment;
using ReviewApi.Domain.Dto;

namespace ReviewApi.Application.Interfaces
{
    public interface ICommentService
    {
        Task<IdResponseModel> Create(string reviewId, string userId, CreateCommentRequestModel model);
        Task<PaginationResponseModel<CommentResponseModel>> GetAllFromReview(string reviewId, PaginationDTO pagination);
        Task Delete(string commentId, string userId);
        Task<CommentResponseModel> GetById(string commentId);
        Task Update(string commentId, string userId, CreateCommentRequestModel model);
    }
}
