using System.Threading.Tasks;
using FluentValidation;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models.User;
using ReviewApi.Application.Validators;
using ReviewApi.Application.Validators.Extensions;
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
        private readonly IEmailUtils _emailUtils;

        public UserService(IUserRepository userRepository, IConfirmationCodeUtils confirmationCodeUtils, IHashUtils hashUtils, IEmailUtils emailUtils)
        {
            _userRepository = userRepository;
            _confirmationCodeUtils = confirmationCodeUtils;
            _hashUtils = hashUtils;
            _emailUtils = emailUtils;
        }

        public async Task Create(CreateUserRequestModel model)
        {
            await new CreateUserValidator().ValidateRequestModelAndThrow(model);

            User user = new User(model.Name, model.Email, model.Password);
            if (await _userRepository.AlreadyExists(user.Email))
            {
                throw new AlreadyExistsException("user");
            }

            user.UpdateConfirmationCode(_confirmationCodeUtils.GenerateConfirmationCode());
            user.UpdatePassword(_hashUtils.GenerateHash(user.Password));

            await _userRepository.Create(user);
            await _userRepository.Save();

            await _emailUtils.SendEmail(user.Email, "Confirmation", user.ConfirmationCode);
        }
    }
}
