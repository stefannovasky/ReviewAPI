using System;
using System.IO;
using System.Threading.Tasks;
using NSubstitute;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models.User;
using ReviewApi.Application.Services;
using ReviewApi.Domain.Dto;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Exceptions;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Shared.Interfaces;
using Xunit;

namespace ReviewApi.UnitTests.Application.Services
{
    public class UserServiceTest
    {
        private readonly IUserService _userService;
        private readonly IRandomCodeUtils _randomCodeUtils;
        private readonly IHashUtils _hashUtilsMock;
        private readonly IEmailUtils _emailUtilsMock;
        private readonly IUserRepository _userRepositoryMock;
        private readonly IImageRepository _imageRepositoryMock;
        private readonly IJwtTokenUtils _jwtTokenUtilsMock;
        private readonly IFileUploadUtils _fileUploadUtilsMock;
        private readonly User _fakeNotConfirmedInsertedUser;
        private readonly User _fakeConfirmedInsertedUser;

        public UserServiceTest()
        {
            _userRepositoryMock = NSubstitute.Substitute.For<IUserRepository>();
            _imageRepositoryMock = NSubstitute.Substitute.For<IImageRepository>();
            _randomCodeUtils = NSubstitute.Substitute.For<IRandomCodeUtils>();
            _hashUtilsMock = NSubstitute.Substitute.For<IHashUtils>();
            _emailUtilsMock = NSubstitute.Substitute.For<IEmailUtils>();
            _jwtTokenUtilsMock = NSubstitute.Substitute.For<IJwtTokenUtils>();
            _fileUploadUtilsMock = NSubstitute.Substitute.For<IFileUploadUtils>();
            _userService = new UserService(_userRepositoryMock, _imageRepositoryMock, _randomCodeUtils, _hashUtilsMock, _emailUtilsMock, _jwtTokenUtilsMock, _fileUploadUtilsMock);

            _fakeNotConfirmedInsertedUser = new User("Fake User", "fake_user@mail.com", "fake password");
            _fakeConfirmedInsertedUser = new User("Fake User", "fake_user@mail.com", "fake password");
            _fakeConfirmedInsertedUser.Confirm();
            _fakeConfirmedInsertedUser.AddImage(new Image("FILENAME", "FILEPATH"));
        }

        [Fact]
        public async Task ShouldThrowAlreadyExistsExceptionOnCreateAlreadyConfirmedEmailUser()
        {
            CreateUserRequestModel model = new CreateUserRequestModel() { Email = _fakeConfirmedInsertedUser.Email, Name = "user name", Password = "user password" };
            _userRepositoryMock.GetByEmail(Arg.Any<string>()).Returns(_fakeConfirmedInsertedUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.Create(model));

            Assert.IsType<AlreadyExistsException>(exception);
            Assert.Contains("user email already exists", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowUserNotConfirmedExceptionOnCreateAlreadyExistsEmailUserNotConfirmed()
        {
            CreateUserRequestModel model = new CreateUserRequestModel() { Email = _fakeConfirmedInsertedUser.Email, Name = "user name", Password = "user password" };
            _userRepositoryMock.GetByEmail(Arg.Any<string>()).Returns(_fakeNotConfirmedInsertedUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.Create(model));

            Assert.IsType<UserNotConfirmedException>(exception);
            Assert.Contains("user already registered, needs to be confirmed.", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowAlreadyExistsExceptionOnCreateAlreadyConfirmedNameUser()
        {
            CreateUserRequestModel model = new CreateUserRequestModel() { Email = "notexistsemail@mail.com", Name = _fakeConfirmedInsertedUser.Name, Password = "user password" };
            _userRepositoryMock.GetByEmail(Arg.Any<string>()).Returns(null as User);
            _userRepositoryMock.GetByName(Arg.Any<string>()).Returns(_fakeConfirmedInsertedUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.Create(model));

            Assert.IsType<AlreadyExistsException>(exception);
            Assert.Contains("user name already exists", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowUserNotConfirmedExceptionOnCreateAlreadyExistsNameUserNotConfirmed()
        {
            CreateUserRequestModel model = new CreateUserRequestModel() { Email = "useremail@mail.com", Name = _fakeNotConfirmedInsertedUser.Name, Password = "user password" };
            _userRepositoryMock.GetByEmail(Arg.Any<string>()).Returns(null as User);
            _userRepositoryMock.GetByName(Arg.Any<string>()).Returns(_fakeNotConfirmedInsertedUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.Create(model));

            Assert.IsType<UserNotConfirmedException>(exception);
            Assert.Contains("user already registered, needs to be confirmed.", exception.Message);
        }

        [Fact]
        public async Task ShouldCreateUser()
        {
            CreateUserRequestModel model = new CreateUserRequestModel() { Email = "user@mail.com", Name = "user name", Password = "user password" };
            _userRepositoryMock.GetByEmail(Arg.Any<string>()).Returns(null as User);
            _fileUploadUtilsMock.GetDefaultUserProfileImage().Returns(new MemoryStream());
            _fileUploadUtilsMock.UploadImage(Arg.Any<Stream>()).Returns(new FileDTO() { FileName = "FILENAME", FilePath = "FILEPATH" });

            Exception exception = await Record.ExceptionAsync(() => _userService.Create(model));

            Assert.Null(exception);

            _randomCodeUtils.Received(1).GenerateRandomCode();
            _hashUtilsMock.Received(1).GenerateHash(Arg.Is<string>(text => text == model.Password));
            await _fileUploadUtilsMock.Received(1).UploadImage(Arg.Any<Stream>());
            await _userRepositoryMock.Received(1).Create(Arg.Is<User>(user => user.Email == model.Email));
            await _userRepositoryMock.Received(2).Save();
            _userRepositoryMock.Received(1).Update(Arg.Is<User>(user => user.Email == model.Email));
            await _imageRepositoryMock.Received(1).Create(Arg.Any<Image>());
            await _imageRepositoryMock.Received(1).Save();
            await _emailUtilsMock.Received(1).SendEmail(Arg.Is<string>(email => email == model.Email), Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundOnTryConfirmNotExistsUser()
        {
            ConfirmUserRequestModel model = new ConfirmUserRequestModel() { Code = "AAAAAAAA" };
            User notExistsUser = null;
            _userRepositoryMock.GetByConfirmationCode(Arg.Any<string>()).Returns(notExistsUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.ConfirmUser(model));

            Assert.IsType<ResourceNotFoundException>(exception);
            Assert.Contains("user not found.", exception.Message);
        }

        [Fact]
        public async Task ShouldConfirmUser()
        {
            ConfirmUserRequestModel model = new ConfirmUserRequestModel() { Code = "AAAAAAAA" };
            User alreadyExistsUser = new User();
            _userRepositoryMock.GetByConfirmationCode(Arg.Any<string>()).Returns(alreadyExistsUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.ConfirmUser(model));

            Assert.Null(exception);
            _userRepositoryMock.Received(1).Update(Arg.Any<User>());
            await _userRepositoryMock.Received(1).Save();
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundExceptionOnAuthenticateNotExistsUser()
        {
            User notExistsUser = null;
            AuthenticationUserRequestModel model = new AuthenticationUserRequestModel()
            {
                Email = "user@mail.com",
                Password = "user password"
            };
            _userRepositoryMock.GetByEmail(Arg.Is<string>(email => email == model.Email)).Returns(notExistsUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.Authenticate(model));

            Assert.IsType<ResourceNotFoundException>(exception);
        }

        [Fact]
        public async Task ShouldThrowUserNotConfirmedExceptionOnAuthenticateNotConfirmedUser()
        {
            AuthenticationUserRequestModel model = new AuthenticationUserRequestModel()
            {
                Email = _fakeNotConfirmedInsertedUser.Email,
                Password = "user password"
            };
            _userRepositoryMock.GetByEmail(Arg.Is<string>(email => email == model.Email)).Returns(_fakeNotConfirmedInsertedUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.Authenticate(model));

            Assert.IsType<UserNotConfirmedException>(exception);
        }

        [Fact]
        public async Task ShouldThrowInvalidPasswordExceptionOnAuthenticateUserWithIncorrectPassword()
        {
            AuthenticationUserRequestModel model = new AuthenticationUserRequestModel()
            {
                Email = _fakeConfirmedInsertedUser.Email,
                Password = "incorrect password"
            };
            _userRepositoryMock.GetByEmail(Arg.Is<string>(email => email == model.Email)).Returns(_fakeConfirmedInsertedUser);
            _hashUtilsMock.CompareHash(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            Exception exception = await Record.ExceptionAsync(() => _userService.Authenticate(model));

            Assert.IsType<InvalidPasswordException>(exception);
        }

        [Fact]
        public async Task ShouldAuthenticateUser()
        {
            AuthenticationUserRequestModel model = new AuthenticationUserRequestModel()
            {
                Email = _fakeConfirmedInsertedUser.Email,
                Password = _fakeConfirmedInsertedUser.Password
            };
            _userRepositoryMock.GetByEmail(Arg.Is<string>(email => email == model.Email)).Returns(_fakeConfirmedInsertedUser);
            _hashUtilsMock.CompareHash(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            Exception exception = await Record.ExceptionAsync(() => _userService.Authenticate(model));

            Assert.Null(exception);
            _jwtTokenUtilsMock.Received(1).GenerateToken(Arg.Any<string>());
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundOnTryUpdateNotExistsUserName()
        {
            Guid notExistsUserGuid = Guid.NewGuid();
            User notExistsUser = null;
            UpdateNameUserRequestModel model = new UpdateNameUserRequestModel() { Name = "User Name" };
            _userRepositoryMock.GetById(Arg.Is<Guid>(id => id == notExistsUserGuid)).Returns(notExistsUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.UpdateUserName(notExistsUserGuid.ToString(), model));

            Assert.IsType<ResourceNotFoundException>(exception);
        }

        [Fact]
        public async Task ShouldUpdateUserName()
        {
            UpdateNameUserRequestModel model = new UpdateNameUserRequestModel() { Name = "User Name" };
            _userRepositoryMock.GetById(Arg.Is<Guid>(id => id == _fakeConfirmedInsertedUser.Id)).Returns(_fakeConfirmedInsertedUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.UpdateUserName(_fakeConfirmedInsertedUser.Id.ToString(), model));

            Assert.Null(exception);
            _userRepositoryMock.Received(1).Update(Arg.Any<User>());
            await _userRepositoryMock.Received(1).Save();
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundExceptionOnTryUpdateNotExistsUserPassword()
        {
            User notExistsUser = null;
            UpdatePasswordUserRequestModel model = new UpdatePasswordUserRequestModel()
            {
                OldPassword = "old password",
                NewPassword = "new password",
                NewPasswordConfirmation = "new password"
            };
            _userRepositoryMock.GetById(Arg.Any<Guid>()).Returns(notExistsUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.UpdatePassword(Guid.NewGuid().ToString(), model));

            Assert.IsType<ResourceNotFoundException>(exception);
        }

        [Fact]
        public async Task ShouldInvalidPasswordExceptionOnTryUpdateUserPasswordWithIncorrectOldPassword()
        {
            UpdatePasswordUserRequestModel model = new UpdatePasswordUserRequestModel()
            {
                OldPassword = "incorrect password",
                NewPassword = "new password",
                NewPasswordConfirmation = "new password"
            };
            _userRepositoryMock.GetById(Arg.Any<Guid>()).Returns(_fakeConfirmedInsertedUser);
            _hashUtilsMock.CompareHash(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            Exception exception = await Record.ExceptionAsync(() => _userService.UpdatePassword(_fakeConfirmedInsertedUser.Id.ToString(), model));

            Assert.IsType<InvalidPasswordException>(exception);
        }

        [Fact]
        public async Task ShouldUpdateUserPassword()
        {
            UpdatePasswordUserRequestModel model = new UpdatePasswordUserRequestModel()
            {
                OldPassword = "new password",
                NewPassword = "new password",
                NewPasswordConfirmation = "new password"
            };
            _userRepositoryMock.GetById(Arg.Any<Guid>()).Returns(_fakeConfirmedInsertedUser);
            _hashUtilsMock.CompareHash(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            Exception exception = await Record.ExceptionAsync(() => _userService.UpdatePassword(_fakeConfirmedInsertedUser.Id.ToString(), model));

            Assert.Null(exception);
            await _userRepositoryMock.Received(1).Save();
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundExceptionOnTryDeleteUser()
        {
            User notExistsUser = null;
            DeleteUserRequestModel model = new DeleteUserRequestModel() { Password = "user password", PasswordConfirmation = "user password" };
            _userRepositoryMock.GetById(Arg.Any<Guid>()).Returns(notExistsUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.Delete(Guid.NewGuid().ToString(), model));

            Assert.IsType<ResourceNotFoundException>(exception);
        }

        [Fact]
        public async Task ShouldThrowInvalidPasswordExceptionOnTryDeleteUser()
        {
            Guid userId = Guid.NewGuid();
            DeleteUserRequestModel model = new DeleteUserRequestModel() { Password = "user password", PasswordConfirmation = "user password" };
            _userRepositoryMock.GetById(Arg.Any<Guid>()).Returns(_fakeConfirmedInsertedUser);
            _hashUtilsMock.CompareHash(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            Exception exception = await Record.ExceptionAsync(() => _userService.Delete(userId.ToString(), model));

            Assert.IsType<InvalidPasswordException>(exception);
        }

        [Fact]
        public async Task ShouldDeleteUser()
        {
            Guid userId = Guid.NewGuid();
            DeleteUserRequestModel model = new DeleteUserRequestModel() { Password = "user password", PasswordConfirmation = "user password" };
            _userRepositoryMock.GetById(Arg.Any<Guid>()).Returns(_fakeConfirmedInsertedUser);
            _hashUtilsMock.CompareHash(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            Exception exception = await Record.ExceptionAsync(() => _userService.Delete(userId.ToString(), model));

            Assert.Null(exception);
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundExceptionOnCallForgotPasswordWithNotExistsUser()
        {
            User notExistsUser = null;
            ForgotPasswordUserRequestModel model = new ForgotPasswordUserRequestModel() { Email = "user@mail.com" };
            _userRepositoryMock.GetByEmail(Arg.Any<string>()).Returns(notExistsUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.ForgotPassword(model));

            Assert.IsType<ResourceNotFoundException>(exception);
        }

        [Fact]
        public async Task ShouldThrowUserNotConfirmedExceptionOnOnCallForgotPasswordWithNotConfirmedUser()
        {
            ForgotPasswordUserRequestModel model = new ForgotPasswordUserRequestModel() { Email = _fakeNotConfirmedInsertedUser.Email };
            _userRepositoryMock.GetByEmail(Arg.Any<string>()).Returns(_fakeNotConfirmedInsertedUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.ForgotPassword(model));

            Assert.IsType<UserNotConfirmedException>(exception);
        }

        [Fact]
        public async Task ShouldCallForgotPasswordUserWithSuccess()
        {
            ForgotPasswordUserRequestModel model = new ForgotPasswordUserRequestModel() { Email = _fakeConfirmedInsertedUser.Email };
            _userRepositoryMock.GetByEmail(Arg.Any<string>()).Returns(_fakeConfirmedInsertedUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.ForgotPassword(model));

            _randomCodeUtils.Received(1).GenerateRandomCode();
            _userRepositoryMock.Received(1).Update(Arg.Any<User>());
            await _userRepositoryMock.Received(1).Save();
            Assert.Null(exception);
        }

        [Fact]
        public async Task ShouldThrowResourceUserNotFoundExceptionOnTryResetPasswordOnNotExistsUser()
        {
            User notExistsUser = null;
            ResetPasswordUserRequestModel model = new ResetPasswordUserRequestModel() { Code = "AAAAAAAA", NewPassword = "NEWPASSWORD", NewPasswordConfirmation = "NEWPASSWORD" };
            _userRepositoryMock.GetByResetPasswordCode(Arg.Any<string>()).Returns(notExistsUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.ResetPassword(model));

            Assert.IsType<ResourceNotFoundException>(exception);
        }

        [Fact]
        public async Task ShouldThrowUserNotConfirmedExceptionOnTryResetPasswordWithNotConfirmedUser()
        {
            ResetPasswordUserRequestModel model = new ResetPasswordUserRequestModel() { Code = "AAAAAAAA", NewPassword = "NEWPASSWORD", NewPasswordConfirmation = "NEWPASSWORD" };
            _userRepositoryMock.GetByResetPasswordCode(Arg.Any<string>()).Returns(_fakeNotConfirmedInsertedUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.ResetPassword(model));

            Assert.IsType<UserNotConfirmedException>(exception);
        }

        [Fact]
        public async Task ShouldResetUserPasswordWithSuccess()
        {
            ResetPasswordUserRequestModel model = new ResetPasswordUserRequestModel() { Code = "AAAAAAAA", NewPassword = "NEWPASSWORD", NewPasswordConfirmation = "NEWPASSWORD" };
            _userRepositoryMock.GetByResetPasswordCode(Arg.Any<string>()).Returns(_fakeConfirmedInsertedUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.ResetPassword(model));
            _hashUtilsMock.Received(1).GenerateHash(Arg.Is<string>(text => text == model.NewPassword));
            _userRepositoryMock.Received(1).Update(Arg.Any<User>());
            await _userRepositoryMock.Received(1).Save();

            Assert.Null(exception);
        }

        [Fact]
        public async Task ShouldGetUserProfile()
        {
            _userRepositoryMock.GetByIdIncludingImage(Arg.Is<Guid>(id => id == _fakeConfirmedInsertedUser.Id)).Returns(_fakeConfirmedInsertedUser);

            UserProfileResponseModel response = await _userService.GetProfile(_fakeConfirmedInsertedUser.Id.ToString());

            Assert.Equal(_fakeConfirmedInsertedUser.Name, response.Name);
            Assert.Equal(_fakeConfirmedInsertedUser.Email, response.Email);
            _fileUploadUtilsMock.Received(1).GenerateImageUrl(Arg.Any<string>());
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundExceptionOnTryGetProfileOfNotExistsUser()
        {
            _userRepositoryMock.GetById(Arg.Any<Guid>()).Returns(null as User);

            Exception exception = await Record.ExceptionAsync(() => _userService.GetProfile(Guid.NewGuid().ToString()));

            Assert.IsType<ResourceNotFoundException>(exception);
        }

        [Fact]
        public async Task ShouldThrowUserNotConfirmedExceptionOnTryGetProfileOfNotConfirmedUser()
        {
            _userRepositoryMock.GetByIdIncludingImage(Arg.Any<Guid>()).Returns(_fakeNotConfirmedInsertedUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.GetProfile(Guid.NewGuid().ToString()));

            Assert.IsType<UserNotConfirmedException>(exception);
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundExceptionOnTryUpdateProfileImageInNotExistsUser()
        {
            _userRepositoryMock.GetByIdIncludingImage(Arg.Any<Guid>()).Returns(null as User);

            Exception exception = await Record.ExceptionAsync(() => _userService.UpdateProfileImage(Guid.NewGuid().ToString(), new MemoryStream()));

            Assert.IsType<ResourceNotFoundException>(exception);
        }

        [Fact]
        public async Task ShouldThrowUserNotConfirmedExceptionOnTryUpdateProfileImageNotConfirmedUser()
        {
            _userRepositoryMock.GetByIdIncludingImage(Arg.Is<Guid>(id => id == _fakeNotConfirmedInsertedUser.Id)).Returns(_fakeNotConfirmedInsertedUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.UpdateProfileImage(_fakeNotConfirmedInsertedUser.Id.ToString(), new MemoryStream()));

            Assert.IsType<UserNotConfirmedException>(exception);
        }

        [Fact]
        public async Task ShouldUpdateProfileImage()
        {
            _userRepositoryMock.GetByIdIncludingImage(Arg.Is<Guid>(id => id == _fakeConfirmedInsertedUser.Id)).Returns(_fakeConfirmedInsertedUser);
            _fileUploadUtilsMock.UploadImage(Arg.Any<Stream>()).Returns(new FileDTO() { FileName = "FILENAME", FilePath = "FILEPATH" });

            Exception exception = await Record.ExceptionAsync(() => _userService.UpdateProfileImage(_fakeConfirmedInsertedUser.Id.ToString(), new MemoryStream()));

            Assert.Null(exception);
            await _fileUploadUtilsMock.Received(1).UploadImage(Arg.Any<Stream>());
            _imageRepositoryMock.Received(1).Update(Arg.Any<Image>());
            await _userRepositoryMock.Received(1).Save();
            await _imageRepositoryMock.Received(1).Save();
        }
    }
}
