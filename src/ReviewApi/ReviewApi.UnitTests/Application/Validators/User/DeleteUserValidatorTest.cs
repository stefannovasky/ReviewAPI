using FluentValidation.TestHelper;
using ReviewApi.Application.Models.User;
using ReviewApi.Application.Validators.User;
using Xunit;

namespace ReviewApi.UnitTests.Application.Validators.User
{
    public class DeleteUserValidatorTest
    {
        private readonly DeleteUserValidator _validator;
        public DeleteUserValidatorTest()
        {
            _validator = new DeleteUserValidator();
        }

        [Theory]
        [InlineData("aaaaaaa")]
        [InlineData("aa")]
        [InlineData("a")]
        public void ShouldHaveValidationErrorWithLessThan8CharactersPassword(string password)
        {
            DeleteUserRequestModel requestModel = new DeleteUserRequestModel()
            {
                Password = password
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Password, requestModel).WithErrorMessage("password must be greater than 8 characters.");
        }

        [Theory]
        [InlineData("greather than 60 characters greather than 60 characters greather than 60 characters greather than 60 characters greather than 60 characters ")]
        public void ShouldHaveValidationErrorWithGreaterThan60CharactersPassword(string password)
        {
            DeleteUserRequestModel requestModel = new DeleteUserRequestModel()
            {
                Password = password
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Password, requestModel).WithErrorMessage("password must be less than 60 characters.");
        }

        [Theory]
        [InlineData(null)]
        public void ShouldHaveValidationErrorWithNullPassword(string password)
        {
            DeleteUserRequestModel requestModel = new DeleteUserRequestModel()
            {
                Password = password
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Password, requestModel).WithErrorMessage("password cannot be null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        public void ShouldHaveValidationErrorWithEmptyPassword(string password)
        {
            DeleteUserRequestModel requestModel = new DeleteUserRequestModel()
            {
                Password = password
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Password, requestModel).WithErrorMessage("password cannot be empty.");
        }

        [Theory]
        [InlineData("password", "notEqualPassword")]
        public void ShouldHaveValidationErrorWithNotEqualPassword(string password, string passwordConfirmation)
        {
            DeleteUserRequestModel requestModel = new DeleteUserRequestModel()
            {
                Password = password,
                PasswordConfirmation = passwordConfirmation
            };

            _validator.ShouldHaveValidationErrorFor(r => r.PasswordConfirmation, requestModel).WithErrorMessage("password confirmation must be equal to new password.");
        }

        [Theory]
        [InlineData("password", "password")]
        public void ShouldNotHaveValidationErrorWithEqualPassword(string password, string passwordConfirmation)
        {
            DeleteUserRequestModel requestModel = new DeleteUserRequestModel()
            {
                Password = password,
                PasswordConfirmation = passwordConfirmation
            };

            _validator.ShouldNotHaveValidationErrorFor(r => r.PasswordConfirmation, requestModel);
        }
    }
}
