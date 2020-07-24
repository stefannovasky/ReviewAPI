﻿using System;
using System.Threading.Tasks;
using NSubstitute;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models.User;
using ReviewApi.Application.Services;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Exceptions;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Shared.Interfaces;
using ReviewApi.Shared.Utils;
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
        private readonly User _fakeNotConfirmedInsertedUser;
        private readonly User _fakeConfirmedInsertedUser;

        public UserServiceTest()
        {
            _userRepositoryMock = NSubstitute.Substitute.For<IUserRepository>();
            _confirmationCodeUtilsMock = NSubstitute.Substitute.For<IConfirmationCodeUtils>();
            _hashUtilsMock = NSubstitute.Substitute.For<IHashUtils>();
            _emailUtilsMock = NSubstitute.Substitute.For<IEmailUtils>();
            _userService = new UserService(_userRepositoryMock, _confirmationCodeUtilsMock, _hashUtilsMock, _emailUtilsMock);

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
    }
}
