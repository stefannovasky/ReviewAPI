using System;
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
            Assert.Null(user.ProfileImage);
            Assert.False(user.Confirmed);
            Assert.Equal(Guid.Empty.ToString(), user.Id.ToString());
            Assert.Null(user.ResetPasswordCode);
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
            Assert.Null(user.ResetPasswordCode);
            Assert.Null(user.ProfileImage);
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
            Assert.Null(user.ResetPasswordCode);
            Assert.Null(user.ConfirmationCode);
            Assert.Null(user.ProfileImage);
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
            Assert.Null(user.ResetPasswordCode);
            Assert.Null(user.ConfirmationCode);
            Assert.Null(user.ProfileImage);
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

        [Fact]
        public void ShouldUpdateName()
        {
            string userName = "User Name";
            User user = new User();

            user.UpdateName(userName);

            Assert.Equal(userName, user.Name);
        }

        [Fact]
        public void ShouldInsertResetPasswordCode()
        {
            string code = "AAAAAAAA";
            User user = new User();

            user.UpdateResetPasswordCode(code);

            Assert.Equal(code, user.ResetPasswordCode);
        }

        [Fact]
        public void ShouldResetPassword()
        {
            string code = "AAAAAAAA";
            string newPasswordHash = "asdhjaudhaiduasd";
            User user = new User();

            user.UpdateResetPasswordCode(code);
            user.ResetPassword(newPasswordHash);

            Assert.NotEqual(code, user.ResetPasswordCode);
            Assert.Equal(newPasswordHash, user.Password);
        }

        
        [Fact]
        public void ShouldUpdateImage()
        {
            ProfileImage image = new ProfileImage("FILENAME", "FILEPATH");
            User user = new User();
            user.AddProfileImage(image);

            Assert.Equal(image.FileName, user.ProfileImage.FileName);
            Assert.Equal(image.FilePath, user.ProfileImage.FilePath);
        }
    }
}
