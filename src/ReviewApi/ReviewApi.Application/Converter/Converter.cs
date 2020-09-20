using ReviewApi.Application.Models.Comment;
using ReviewApi.Application.Models.Review;
using ReviewApi.Application.Models.User;
using ReviewApi.Domain.Entities;
using ReviewApi.Shared.Interfaces;

namespace ReviewApi.Application.Converter
{
    public class Converter : IConverter
    {
        private readonly IFileUploadUtils _fileUploadUtils;

        public Converter(IFileUploadUtils fileUploadUtils)
        {
            _fileUploadUtils = fileUploadUtils;
        }

        public ReviewResponseModel ConvertReviewToReviewResponseModel(Review review)
        {
            string reviewImageUrl = _fileUploadUtils.GenerateImageUrl(review.Image.FileName);
            return new ReviewResponseModel()
            {
                Id = review.Id,
                Image = reviewImageUrl,
                Creator = review.Creator.Name,
                Stars = review.Stars,
                Text = review.Text,
                Title = review.Title
            };
        }

        public UserProfileResponseModel ConvertUserToUserProfileResponseModel(User user)
        {
            return new UserProfileResponseModel()
            {
                Email = user.Email,
                Name = user.Name,
                Image = _fileUploadUtils.GenerateImageUrl(user.ProfileImage.FileName)
            };
        }

        public ReviewResponseModel ConvertReviewToReviewResponseModel(Review review, string userName)
        {
            return new ReviewResponseModel()
            {
                Creator = userName,
                Id = review.Id,
                Image = _fileUploadUtils.GenerateImageUrl(review.Image.FileName),
                Stars = review.Stars,
                Text = review.Text,
                Title = review.Title
            };
        }

        public CommentResponseModel ConvertCommentToCommentResponseModel(Comment comment) 
        {
            return new CommentResponseModel()
            {
                Id = comment.Id,
                Text = comment.Text,
                User = new UserProfileResponseModel()
                {
                    Email = comment?.User.Email,
                    Name = comment?.User.Name,
                    Image = _fileUploadUtils.GenerateImageUrl(comment?.User?.ProfileImage.FileName)
                }
            };
        }
    }
}
