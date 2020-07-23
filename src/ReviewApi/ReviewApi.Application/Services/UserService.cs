using System.Threading.Tasks;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models.User;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Exceptions;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Shared.Interfaces;

namespace ReviewApi.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfirmationCodeUtils _confirmationCodeUtils;
        private readonly IHashUtils _hashUtils;

        public UserService(IUserRepository userRepository, IConfirmationCodeUtils confirmationCodeUtils, IHashUtils hashUtils)
        {
            _userRepository = userRepository;
            _confirmationCodeUtils = confirmationCodeUtils;
            _hashUtils = hashUtils;
        }

        public async Task Create(CreateUserRequestModel model)
        {
            User user = new User(model.Name, model.Email, model.Password);
            if (await _userRepository.AlreadyExists(user.Email))
            {
                throw new AlreadyExistsException("user");
            }
            user.UpdateConfirmationCode(_confirmationCodeUtils.GenerateConfirmationCode());
            user.UpdatePassword(_hashUtils.GenerateHash(user.Password));
            await _userRepository.Create(user);
            await _userRepository.Save();
        }
    }
}
