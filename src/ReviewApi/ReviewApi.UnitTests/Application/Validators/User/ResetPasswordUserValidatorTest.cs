using FluentValidation.TestHelper;
using ReviewApi.Application.Models.User;
using ReviewApi.Application.Validators.User;
using Xunit;

namespace ReviewApi.UnitTests.Application.Validators.User
{
    public class ResetPasswordUserValidatorTest
    {
        private readonly ResetPasswordUserValidator _validator;

        public ResetPasswordUserValidatorTest()
        {
            _validator = new ResetPasswordUserValidator(); 
        }


        [Theory]
        [InlineData(null)]
        public void ShouldHaveValidationErrorWithNullCode(string code)
        {
            ResetPasswordUserRequestModel requestModel = new ResetPasswordUserRequestModel() { Code = code };

            _validator.ShouldHaveValidationErrorFor(p => p.Code, requestModel).WithErrorMessage("code cannot be null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ShouldHaveValidationErrorWithEmptyCode(string code)
        {
            ResetPasswordUserRequestModel requestModel = new ResetPasswordUserRequestModel() { Code = code };

            _validator.ShouldHaveValidationErrorFor(p => p.Code, requestModel).WithErrorMessage("code cannot be empty.");
        }

        [Theory]
        [InlineData("1")]
        [InlineData("1234567")]
        [InlineData("123456789")]
        public void ShouldHaveValidationErrorWithInvalidLengthCode(string code)
        {
            ResetPasswordUserRequestModel requestModel = new ResetPasswordUserRequestModel() { Code = code };

            _validator.ShouldHaveValidationErrorFor(p => p.Code, requestModel).WithErrorMessage("code must be 8 characters.");
        }

        [Theory]
        [InlineData("aaaaaaa")]
        [InlineData("aa")]
        [InlineData("a")]
        public void ShouldHaveValidationErrorWithLessThan8CharactersOldPassword(string newPassword)
        {
            ResetPasswordUserRequestModel requestModel = new ResetPasswordUserRequestModel()
            {
                NewPassword = newPassword
            };

            _validator.ShouldHaveValidationErrorFor(r => r.NewPassword, requestModel).WithErrorMessage("new password must be greater than 8 characters.");
        }

        [Theory]
        [InlineData("greather than 60 characters greather than 60 characters greather than 60 characters greather than 60 characters greather than 60 characters ")]
        public void ShouldHaveValidationErrorWithGreaterThan60CharactersOldPassword(string newPassword)
        {
            ResetPasswordUserRequestModel requestModel = new ResetPasswordUserRequestModel()
            {
                NewPassword = newPassword
            };

            _validator.ShouldHaveValidationErrorFor(r => r.NewPassword, requestModel).WithErrorMessage("new password must be less than 60 characters.");
        }

        [Theory]
        [InlineData(null)]
        public void ShouldHaveValidationErrorWithNullOldPassword(string newPassword)
        {
            ResetPasswordUserRequestModel requestModel = new ResetPasswordUserRequestModel()
            {
                NewPassword = newPassword
            };

            _validator.ShouldHaveValidationErrorFor(r => r.NewPassword, requestModel).WithErrorMessage("new password cannot be null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        public void ShouldHaveValidationErrorWithEmptyOldPassword(string newPassword)
        {
            ResetPasswordUserRequestModel requestModel = new ResetPasswordUserRequestModel()
            {
                NewPassword = newPassword
            };

            _validator.ShouldHaveValidationErrorFor(r => r.NewPassword, requestModel).WithErrorMessage("new password cannot be empty.");
        }

        [Theory]
        [InlineData("password", "notEqualPassword")]
        public void ShouldHaveValidationErrorWithNotEqualNewPassword(string newPassword, string newPasswordConfirmation)
        {
            ResetPasswordUserRequestModel requestModel = new ResetPasswordUserRequestModel()
            {
                NewPassword = newPassword,
                NewPasswordConfirmation = newPasswordConfirmation
            };

            _validator.ShouldHaveValidationErrorFor(r => r.NewPasswordConfirmation, requestModel).WithErrorMessage("new password confirmation must be equal to new password.");
        }

        [Theory]
        [InlineData("password", "password")]
        public void ShouldNotHaveValidationErrorWithEqualNewPassword(string newPassword, string newPasswordConfirmation)
        {
            ResetPasswordUserRequestModel requestModel = new ResetPasswordUserRequestModel()
            {
                NewPassword = newPassword,
                NewPasswordConfirmation = newPasswordConfirmation
            };

            _validator.ShouldNotHaveValidationErrorFor(r => r.NewPasswordConfirmation, requestModel);
        }
    }
}
