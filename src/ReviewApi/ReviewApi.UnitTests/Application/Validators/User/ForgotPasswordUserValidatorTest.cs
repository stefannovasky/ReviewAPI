using FluentValidation.TestHelper;
using ReviewApi.Application.Models.User;
using ReviewApi.Application.Validators.User;
using Xunit;

namespace ReviewApi.UnitTests.Application.Validators.User
{
    public class ForgotPasswordUserValidatorTest
    {
        private readonly ForgotPasswordUserValidator _validator; 
        public ForgotPasswordUserValidatorTest()
        {
            _validator = new ForgotPasswordUserValidator(); 
        }

        [Theory]
        [InlineData(null)]
        public void ShouldHaveValidationErrorWithNullEmail(string email)
        {
            ForgotPasswordUserRequestModel requestModel = new ForgotPasswordUserRequestModel()
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
            ForgotPasswordUserRequestModel requestModel = new ForgotPasswordUserRequestModel()
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
            ForgotPasswordUserRequestModel requestModel = new ForgotPasswordUserRequestModel()
            {
                Email = email
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Email, requestModel).WithErrorMessage("email must be a valid email.");
        }


        [Theory]
        [InlineData("email.greater.than.244.chars.email.greater.than.244.chars.email.greater.than.244.chars.email.greater.than.244.charsemail.greater.than.244.chars.email.greater.than.244.chars.email.greater.than.244.chars..email.greater.than.244.chars..email.greater.than.244.chars@mail.com")]
        public void ShouldHaveValidationErrorGreaterThan255CharactersEmail(string email)
        {
            ForgotPasswordUserRequestModel requestModel = new ForgotPasswordUserRequestModel()
            {
                Email = email
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Email, requestModel).WithErrorMessage("email must be less than 254 characters.");
        }
    }
}
