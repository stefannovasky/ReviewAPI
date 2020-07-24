using System.Threading.Tasks;
using ReviewApi.Application.Models.User;

namespace ReviewApi.Application.Interfaces
{
    public interface IUserService
    {
        Task Create(CreateUserRequestModel model);
        Task ConfirmUser(ConfirmUserRequestModel model);
        Task<AuthenticationUserResponseModel> Authenticate(AuthenticationUserRequestModel model);
    }
}
