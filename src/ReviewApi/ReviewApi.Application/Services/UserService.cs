using System;
using System.IO;
using System.Threading.Tasks;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models.User;
using ReviewApi.Application.Validators.Extensions;
using ReviewApi.Application.Validators.User;
using ReviewApi.Domain.Dto;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Exceptions;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Shared.Interfaces;

namespace ReviewApi.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IImageRepository _imageRepository;
        private readonly IRandomCodeUtils _randomCodeUtils;
        private readonly IHashUtils _hashUtils;
        private readonly IEmailUtils _emailUtils;
        private readonly IJwtTokenUtils _jwtTokenUtils;
        private readonly IFileUploadUtils _fileUploadUtils;

        public UserService(IUserRepository userRepository, IImageRepository imageRepository, IRandomCodeUtils randomCodeUtils, IHashUtils hashUtils, IEmailUtils emailUtils, IJwtTokenUtils jwtTokenUtils, IFileUploadUtils fileUploadUtils)
        {
            _userRepository = userRepository;
            _imageRepository = imageRepository;
            _randomCodeUtils = randomCodeUtils;
            _hashUtils = hashUtils;
            _emailUtils = emailUtils;
            _jwtTokenUtils = jwtTokenUtils;
            _fileUploadUtils = fileUploadUtils;
        }

        public async Task<AuthenticationUserResponseModel> Authenticate(AuthenticationUserRequestModel model)
        {
            await new AuthenticationUserValidator().ValidateRequestModelAndThrow(model);

            User user = await _userRepository.GetByEmail(model.Email);
            VerifyIfUserIsNullOrNotConfirmedAndThrow(user);

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
            await VerifyIfUserExistsByNameAndThrow(user.Name);
            await VerifyIfUserExistsByEmailAndThrow(user.Email);

            user.UpdateConfirmationCode(_randomCodeUtils.GenerateRandomCode());
            user.UpdatePassword(_hashUtils.GenerateHash(user.Password));

            await _userRepository.Create(user);
            await _userRepository.Save();
            await SetNewRegisteredUserProfileImage(user);

            await _emailUtils.SendEmail(user.Email, "Confirmation", $"Please confirm your account using this code {user.ConfirmationCode}");
        }

        public async Task Delete(string userId, DeleteUserRequestModel model)
        {
            await new DeleteUserValidator().ValidateRequestModelAndThrow(model);

            User user = await _userRepository.GetById(Guid.Parse(userId));
            if (user == null)
            {
                throw new ResourceNotFoundException("user not found.");
            }
            if (!_hashUtils.CompareHash(model.Password, user.Password))
            {
                throw new InvalidPasswordException();
            }

            _userRepository.Delete(user);
            await _userRepository.Save();
        }

        public async Task ForgotPassword(ForgotPasswordUserRequestModel model)
        {
            await new ForgotPasswordUserValidator().ValidateRequestModelAndThrow(model);

            User user = await _userRepository.GetByEmail(model.Email);
            VerifyIfUserIsNullOrNotConfirmedAndThrow(user);

            user.UpdateResetPasswordCode(_randomCodeUtils.GenerateRandomCode());

            _userRepository.Update(user);
            await _userRepository.Save();

            await _emailUtils.SendEmail(user.Email, "Reset Password", $"Reset your password with this code: {user.ResetPasswordCode}");
        }

        public async Task<UserProfileResponseModel> GetProfile(string userId)
        {
            User user = await _userRepository.GetById(Guid.Parse(userId));
            VerifyIfUserIsNullOrNotConfirmedAndThrow(user);

            return new UserProfileResponseModel() { Email = user.Email, Name = user.Name };
        }

        public async Task ResetPassword(ResetPasswordUserRequestModel model)
        {
            await new ResetPasswordUserValidator().ValidateRequestModelAndThrow(model);

            User user = await _userRepository.GetByResetPasswordCode(model.Code);
            VerifyIfUserIsNullOrNotConfirmedAndThrow(user);

            user.ResetPassword(_hashUtils.GenerateHash(model.NewPassword));

            _userRepository.Update(user);
            await _userRepository.Save();
        }

        public async Task UpdatePassword(string userId, UpdatePasswordUserRequestModel model)
        {
            await new UpdatePasswordUserValidator().ValidateRequestModelAndThrow(model);

            User user = await _userRepository.GetById(Guid.Parse(userId));
            if (user == null)
            {
                throw new ResourceNotFoundException("user not found.");
            }
            if (!_hashUtils.CompareHash(model.OldPassword, user.Password))
            {
                throw new InvalidPasswordException();
            }

            user.UpdatePassword(_hashUtils.GenerateHash(model.NewPassword));

            _userRepository.Update(user);
            await _userRepository.Save();
        }

        public async Task UpdateUserName(string userId, UpdateNameUserRequestModel model)
        {
            await new UpdateNameUserValidator().ValidateRequestModelAndThrow(model);

            User user = await _userRepository.GetById(Guid.Parse(userId));
            VerifyIfUserIsNullOrNotConfirmedAndThrow(user);

            user.UpdateName(model.Name);

            _userRepository.Update(user);
            await _userRepository.Save();
        }

        public async Task UpdateProfileImage(string userId, Stream imageStream)
        {
            User user = await _userRepository.GetByIdIncludingImage(Guid.Parse(userId));
            VerifyIfUserIsNullOrNotConfirmedAndThrow(user);

            FileDTO uploadedImage = await _fileUploadUtils.UploadFile(imageStream);
            _imageRepository.Update(new Image(user.ImageId, uploadedImage.FileName, uploadedImage.FilePath, user.Id));

            await _userRepository.Save();
            await _imageRepository.Save();
        }

        private void VerifyIfUserIsNullOrNotConfirmedAndThrow(User user)
        {
            if (user == null)
            {
                throw new ResourceNotFoundException("user not found.");
            }
            if (!user.Confirmed)
            {
                throw new UserNotConfirmedException();
            }
        }

        private async Task VerifyIfUserExistsByNameAndThrow(string userName)
        {
            User userInDatabase = await _userRepository.GetByName(userName);
            if (userInDatabase != null)
            {
                if (!userInDatabase.Confirmed)
                {
                    throw new UserNotConfirmedException();
                }
                throw new AlreadyExistsException("user name");
            }
        }

        private async Task VerifyIfUserExistsByEmailAndThrow(string userEmail)
        {
            User userInDatabase = await _userRepository.GetByEmail(userEmail);
            if (userInDatabase != null)
            {
                if (!userInDatabase.Confirmed)
                {
                    throw new UserNotConfirmedException();
                }
                throw new AlreadyExistsException("user email");
            }
        }

        private async Task SetNewRegisteredUserProfileImage(User registeredUser)
        {
            Stream userDefaultProfileImage = _fileUploadUtils.GetDefaultUserProfileImage();
            FileDTO uploadedProfileImage = await _fileUploadUtils.UploadFile(userDefaultProfileImage);

            Image image = new Image(uploadedProfileImage.FileName, uploadedProfileImage.FilePath, registeredUser.Id);
            await _imageRepository.Create(image);
            await _imageRepository.Save();

            registeredUser.UpdateImageId(image.Id);
            _userRepository.Update(registeredUser);
            await _userRepository.Save();
        }
    }
}
