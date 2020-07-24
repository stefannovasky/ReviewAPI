using System;
using System.Threading.Tasks;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models.User;
using ReviewApi.Application.Validators.Extensions;
using ReviewApi.Application.Validators.User;
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
        private readonly IJwtTokenUtils _jwtTokenUtils;

        public UserService(IUserRepository userRepository, IConfirmationCodeUtils confirmationCodeUtils, IHashUtils hashUtils, IEmailUtils emailUtils, IJwtTokenUtils jwtTokenUtils)
        {
            _userRepository = userRepository;
            _confirmationCodeUtils = confirmationCodeUtils;
            _hashUtils = hashUtils;
            _emailUtils = emailUtils;
            _jwtTokenUtils = jwtTokenUtils;
        }

        public async Task<AuthenticationUserResponseModel> Authenticate(AuthenticationUserRequestModel model)
        {
            await new AuthenticationUserValidator().ValidateRequestModelAndThrow(model);

            User user = await _userRepository.GetByEmail(model.Email);
            if (user == null)
            {
                throw new ResourceNotFoundException("account not found.");
            }
            if (!user.Confirmed)
            {
                throw new UserNotConfirmedException();
            }
            if (!_hashUtils.CompareHash(model.Password, user.Password))
            {
                throw new InvalidPasswordException(); 
            }

            UserResponseModel userResponseModel = new UserResponseModel() { Name = user.Name };
            return new AuthenticationUserResponseModel() { User = userResponseModel, Token = _jwtTokenUtils.GenerateToken(user.Id.ToString()) };
        }

        public async Task ConfirmUser(ConfirmUserRequestModel model)
        {
            await new ConfirmUserValidator().ValidateRequestModelAndThrow(model);

            User user = await _userRepository.GetByConfirmationCode(model.Code);
            if (user == null)
            {
                throw new ResourceNotFoundException("user not found.");
            }

            user.Confirm();

            _userRepository.Update(user);
            await _userRepository.Save();
        }

        public async Task Create(CreateUserRequestModel model)
        {
            await new CreateUserValidator().ValidateRequestModelAndThrow(model);

            User user = new User(model.Name, model.Email, model.Password);
            User userInDatabase = await _userRepository.GetByEmail(user.Email);

            bool userExists = userInDatabase != null;
            if (userExists)
            {
                if (!userInDatabase.Confirmed)
                {
                    throw new UserNotConfirmedException();
                }
                throw new AlreadyExistsException("user");
            }

            user.UpdateConfirmationCode(_confirmationCodeUtils.GenerateConfirmationCode());
            user.UpdatePassword(_hashUtils.GenerateHash(user.Password));

            await _userRepository.Create(user);
            await _userRepository.Save();

            await _emailUtils.SendEmail(user.Email, "Confirmation", $"Please confirm your account using this code {user.ConfirmationCode}");
        }
    }
}
