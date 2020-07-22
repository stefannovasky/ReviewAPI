﻿using System;
using ReviewApi.Domain.Entities;
using Xunit;

namespace ReviewApi.UnitTests.Domain.Entities
{
    public class UserTest
    {
        [Fact]
        public void ShouldConstructUserWithEmptyConstructor()
        {
            User user = new User();

            Assert.Null(user.Name);
            Assert.Null(user.Email);
            Assert.Null(user.Password);
            Assert.Null(user.ConfirmationCode);
            Assert.False(user.Deleted);
            Assert.False(user.Confirmed);
            Assert.Equal(Guid.Empty.ToString(), user.Id.ToString());
        }

        [Fact]
        public void ShouldConstructUserWithOnlyIdConstructor()
        {
            Guid id = Guid.NewGuid();

            User user = new User(id);

            Assert.Equal(id, user.Id);
            Assert.Null(user.Name);
            Assert.Null(user.Email);
            Assert.Null(user.Password);
            Assert.Null(user.ConfirmationCode);
            Assert.False(user.Confirmed);
            Assert.False(user.Deleted);
        }

        [Fact]
        public void ShouldConstructUserWithNameEmailAndPasswordConstructor()
        {
            string userName = "user name";
            string userEmail = "user@mail.com";
            string userPassword = "userPassword";

            User user = new User(userName, userEmail, userPassword);

            Assert.Equal(Guid.Empty, user.Id);
            Assert.Equal(userName, user.Name);
            Assert.Equal(userEmail, user.Email);
            Assert.Equal(userPassword, user.Password);
            Assert.False(user.Confirmed);
            Assert.False(user.Deleted);
            Assert.Null(user.ConfirmationCode);
        }

        [Fact]
        public void ShouldConstructUserWithIdNameEmailAndPasswordConstructor()
        {
            Guid id = Guid.NewGuid();
            string userName = "user name";
            string userEmail = "user@mail.com";
            string userPassword = "userPassword";

            User user = new User(id, userName, userEmail, userPassword);

            Assert.Equal(id, user.Id);
            Assert.Equal(userName, user.Name);
            Assert.Equal(userEmail, user.Email);
            Assert.Equal(userPassword, user.Password);
            Assert.False(user.Confirmed);
            Assert.False(user.Deleted);
            Assert.Null(user.ConfirmationCode);
        }

        [Fact]
        public void ShouldUpdatePassword()
        {
            string password = "password";
            User user = new User();

            user.UpdatePassword(password);

            Assert.Equal(password, user.Password);
        }

        [Fact]
        public void ShouldUpdateConfirmationCode()
        {
            string confirmationCode = "confirmationCode";
            User user = new User();

            user.UpdateConfirmationCode(confirmationCode);

            Assert.Equal(confirmationCode, user.ConfirmationCode);
        }

        [Fact]
        public void ShouldConfirmUser()
        {
            User user = new User();

            user.Confirm();

            Assert.True(user.Confirmed);
        }
    }
}
