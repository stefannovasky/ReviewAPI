using System.Threading.Tasks;
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
        // TODO: Unit and Integration tests
        Task ResetPassword(ResetPasswordUserRequestModel model);
    }
}
