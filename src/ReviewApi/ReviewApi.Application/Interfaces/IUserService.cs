using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ReviewApi.Application.Models.Review;
using ReviewApi.Application.Models.User;

namespace ReviewApi.Application.Interfaces
{
    public interface IUserService
    {
        Task Create(CreateUserRequestModel model);
        Task ConfirmUser(ConfirmUserRequestModel model);
        Task<AuthenticationUserResponseModel> Authenticate(AuthenticationUserRequestModel model);
        Task UpdateUserName(string userId, UpdateNameUserRequestModel model);
        Task UpdatePassword(string userId, UpdatePasswordUserRequestModel model);
        Task Delete(string userId, DeleteUserRequestModel model);
        Task ForgotPassword(ForgotPasswordUserRequestModel model);
        Task ResetPassword(ResetPasswordUserRequestModel model);
        Task<UserProfileResponseModel> GetAuthenticatedUserProfile(string userId);
        Task UpdateProfileImage(string userId, Stream imageStream);
        Task<UserProfileResponseModel> GetProfile(string userName);
        Task<IEnumerable<ReviewResponseModel>> GetUserReviews(string name);
    }
}
