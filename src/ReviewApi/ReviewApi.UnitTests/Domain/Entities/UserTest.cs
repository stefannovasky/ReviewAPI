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
            Assert.False(user.Deleted);
            Assert.Equal(0, user.Id);
        }

        [Fact]
        public void ShouldConstructUserWithOnlyIdConstructor()
        {
            int userId = 1; 

            User user = new User(userId);

            Assert.Equal(userId, user.Id);
            Assert.Null(user.Name);
            Assert.Null(user.Email);
            Assert.Null(user.Password);
            Assert.False(user.Deleted);
        }

        [Fact]
        public void ShouldConstructUserWithNameEmailAndPasswordConstructor()
        {
            string userName = "user name";
            string userEmail = "user@mail.com";
            string userPassword = "userPassword";

            User user = new User(userName, userEmail, userPassword);

            Assert.Equal(0, user.Id);
            Assert.Equal(userName, user.Name);
            Assert.Equal(userEmail, user.Email);
            Assert.Equal(userPassword, user.Password);
            Assert.False(user.Deleted);
        }

        [Fact]
        public void ShouldConstructUserWithIdNameEmailAndPasswordConstructor()
        {
            int userId = 1;
            string userName = "user name";
            string userEmail = "user@mail.com";
            string userPassword = "userPassword";

            User user = new User(userId, userName, userEmail, userPassword);

            Assert.Equal(1, user.Id);
            Assert.Equal(userName, user.Name);
            Assert.Equal(userEmail, user.Email);
            Assert.Equal(userPassword, user.Password);
            Assert.False(user.Deleted);
        }

        [Fact]
        public void ShouldUpdatePassword()
        {
            string password = "password";
            User user = new User();

            user.UpdatePassword(password);

            Assert.Equal(password, user.Password);
        }
    }
}
