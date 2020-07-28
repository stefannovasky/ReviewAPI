using System;
using System.Threading.Tasks;
using NSubstitute;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models.User;
using ReviewApi.Application.Services;
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
        private readonly IConfirmationCodeUtils _confirmationCodeUtilsMock;
        private readonly IHashUtils _hashUtilsMock;
        private readonly IEmailUtils _emailUtilsMock;
        private readonly IUserRepository _userRepositoryMock;
        private readonly IJwtTokenUtils _jwtTokenUtilsMock;
        private readonly User _fakeNotConfirmedInsertedUser;
        private readonly User _fakeConfirmedInsertedUser;

        public UserServiceTest()
        {
            _userRepositoryMock = NSubstitute.Substitute.For<IUserRepository>();
            _confirmationCodeUtilsMock = NSubstitute.Substitute.For<IConfirmationCodeUtils>();
            _hashUtilsMock = NSubstitute.Substitute.For<IHashUtils>();
            _emailUtilsMock = NSubstitute.Substitute.For<IEmailUtils>();
            _jwtTokenUtilsMock = NSubstitute.Substitute.For<IJwtTokenUtils>();
            _userService = new UserService(_userRepositoryMock, _confirmationCodeUtilsMock, _hashUtilsMock, _emailUtilsMock, _jwtTokenUtilsMock);

            _fakeNotConfirmedInsertedUser = new User("Fake User", "fake_user@mail.com", "fake password");
            _fakeConfirmedInsertedUser = new User("Fake User", "fake_user@mail.com", "fake password");
            _fakeConfirmedInsertedUser.Confirm();
        }

        [Fact]
        public async Task ShouldThrowAlreadyExistsExceptionOnCreateAlreadyConfirmedUser()
        {
            CreateUserRequestModel model = new CreateUserRequestModel() { Email = _fakeConfirmedInsertedUser.Email, Name = "user name", Password = "user password" };
            _userRepositoryMock.GetByEmail(Arg.Any<string>()).Returns(_fakeConfirmedInsertedUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.Create(model));

            Assert.IsType<AlreadyExistsException>(exception);
            Assert.Contains("user already exists", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowUserNotConfirmedExceptionOnCreateAlreadyExistsUserNotConfirmed()
        {
            CreateUserRequestModel model = new CreateUserRequestModel() { Email = _fakeConfirmedInsertedUser.Email, Name = "user name", Password = "user password" };
            _userRepositoryMock.GetByEmail(Arg.Any<string>()).Returns(_fakeNotConfirmedInsertedUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.Create(model));

            Assert.IsType<UserNotConfirmedException>(exception);
            Assert.Contains("email already registered, needs to be confirmed.", exception.Message);
        }

        [Fact]
        public async Task ShouldCreateUser()
        {
            User notExistsUser = null;
            CreateUserRequestModel model = new CreateUserRequestModel() { Email = "user@mail.com", Name = "user name", Password = "user password" };
            _userRepositoryMock.GetByEmail(Arg.Any<string>()).Returns(notExistsUser);

            Exception exception = await Record.ExceptionAsync(() => _userService.Create(model));

            Assert.Null(exception);

            _confirmationCodeUtilsMock.Received(1).GenerateConfirmationCode();
            _hashUtilsMock.Received(1).GenerateHash(Arg.Is<string>(text => text == model.Password));
            await _emailUtilsMock.Received(1).SendEmail(Arg.Is<string>(email => email == model.Email), Arg.Any<string>(), Arg.Any<string>());
            await _userRepositoryMock.Received(1).Create(Arg.Is<User>(user => user.Email == model.Email));
            await _userRepositoryMock.Received(1).Save();
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
            Guid userId = Guid.NewGuid();
            User user = new User(userId);
            UpdateNameUserRequestModel model = new UpdateNameUserRequestModel() { Name = "User Name" };
            _userRepositoryMock.GetById(Arg.Is<Guid>(id => id == userId)).Returns(user);

            Exception exception = await Record.ExceptionAsync(() => _userService.UpdateUserName(userId.ToString(), model));

            Assert.Equal(model.Name, user.Name);
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
            Guid userId = Guid.NewGuid();
            User registeredUser = new User(userId, "user name", "user@mail.com", "user password");
            UpdatePasswordUserRequestModel model = new UpdatePasswordUserRequestModel()
            {
                OldPassword = "incorrect password",
                NewPassword = "new password",
                NewPasswordConfirmation = "new password"
            };
            _userRepositoryMock.GetById(Arg.Any<Guid>()).Returns(registeredUser);
            _hashUtilsMock.CompareHash(Arg.Any<string>(), Arg.Any<string>()).Returns(false);

            Exception exception = await Record.ExceptionAsync(() => _userService.UpdatePassword(userId.ToString(), model));

            Assert.IsType<InvalidPasswordException>(exception);
        }

        [Fact]
        public async Task ShouldUpdateUserPassword()
        {
            Guid userId = Guid.NewGuid();
            User registeredUser = new User(userId, "user name", "user@mail.com", "user password");
            UpdatePasswordUserRequestModel model = new UpdatePasswordUserRequestModel()
            {
                OldPassword = "new password",
                NewPassword = "new password",
                NewPasswordConfirmation = "new password"
            };
            _userRepositoryMock.GetById(Arg.Any<Guid>()).Returns(registeredUser);
            _hashUtilsMock.CompareHash(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

            Exception exception = await Record.ExceptionAsync(() => _userService.UpdatePassword(userId.ToString(), model));

            Assert.Null(exception);
            await _userRepositoryMock.Received(1).Save();
        }
    }
}
