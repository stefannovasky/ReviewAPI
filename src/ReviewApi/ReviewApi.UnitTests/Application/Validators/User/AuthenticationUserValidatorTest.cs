using FluentValidation.TestHelper;
using ReviewApi.Application.Models.User;
using ReviewApi.Application.Validators.User;
using Xunit;

namespace ReviewApi.UnitTests.Application.Validators.User
{
    public class AuthenticationUserValidatorTest
    {
        private readonly AuthenticationUserValidator _validator;
        public AuthenticationUserValidatorTest()
        {
            _validator = new AuthenticationUserValidator();
        }

        [Theory]
        [InlineData(null)]
        public void ShouldHaveValidationErrorWithNullEmail(string email)
        {
            AuthenticationUserRequestModel requestModel = new AuthenticationUserRequestModel()
            {
                Email = email
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Email, requestModel).WithErrorMessage("email cannot be null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ShouldHaveValidationErrorWithEmptyEmail(string email)
        {
            AuthenticationUserRequestModel requestModel = new AuthenticationUserRequestModel()
            {
                Email = email
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Email, requestModel).WithErrorMessage("email cannot be empty.");
        }

        [Theory]
        [InlineData("invalid email")]
        [InlineData("Invalid  Email")]
        [InlineData("invalidemail")]
        [InlineData("invalid.email")]
        [InlineData("invalid@email")]
        [InlineData("invalid@email.")]
        [InlineData("invalid.email@")]
        [InlineData("@email.com")]
        [InlineData(".email@com")]
        [InlineData("InvalidEmail@mail.com")]

        public void ShouldHaveValidationErrorWithInvalidEmail(string email)
        {
            AuthenticationUserRequestModel requestModel = new AuthenticationUserRequestModel()
            {
                Email = email
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Email, requestModel).WithErrorMessage("email must be a valid email.");
        }


        [Theory]
        [InlineData("email.greater.than.244.chars.email.greater.than.244.chars.email.greater.than.244.chars.email.greater.than.244.charsemail.greater.than.244.chars.email.greater.than.244.chars.email.greater.than.244.chars..email.greater.than.244.chars..email.greater.than.244.chars@mail.com")]
        public void ShouldHaveValidationErrorGreaterThan255CharactersEmail(string email)
        {
            AuthenticationUserRequestModel requestModel = new AuthenticationUserRequestModel()
            {
                Email = email
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Email, requestModel).WithErrorMessage("email must be less than 254 characters.");
        }

        [Theory]
        [InlineData("aaaaaaa")]
        [InlineData("aa")]
        [InlineData("a")]
        public void ShouldHaveValidationErrorWithLessThan8CharactersPassword(string password)
        {
            AuthenticationUserRequestModel requestModel = new AuthenticationUserRequestModel()
            {
                Password = password
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Password, requestModel).WithErrorMessage("password must be greater than 8 characters.");
        }

        [Theory]
        [InlineData("greather than 60 characters greather than 60 characters greather than 60 characters greather than 60 characters greather than 60 characters ")]
        public void ShouldHaveValidationErrorWithGreaterThan60CharactersPassword(string password)
        {
            AuthenticationUserRequestModel requestModel = new AuthenticationUserRequestModel()
            {
                Password = password
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Password, requestModel).WithErrorMessage("password must be less than 60 characters.");
        }

        [Theory]
        [InlineData(null)]
        public void ShouldHaveValidationErrorWithNullPassword(string password)
        {
            AuthenticationUserRequestModel requestModel = new AuthenticationUserRequestModel()
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
            AuthenticationUserRequestModel requestModel = new AuthenticationUserRequestModel()
            {
                Password = password
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Password, requestModel).WithErrorMessage("password cannot be empty.");
        }
    }
}
