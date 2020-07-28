using FluentValidation.TestHelper;
using ReviewApi.Application.Models.User;
using ReviewApi.Application.Validators.User;
using Xunit;

namespace ReviewApi.UnitTests.Application.Validators.User
{
    public class UpdatePasswordUserValidatorTest
    {
        private readonly UpdatePasswordUserValidator _validator; 
        public UpdatePasswordUserValidatorTest()
        {
            _validator = new UpdatePasswordUserValidator(); 
        }

        [Theory]
        [InlineData("aaaaaaa")]
        [InlineData("aa")]
        [InlineData("a")]
        public void ShouldHaveValidationErrorWithLessThan8CharactersOldPassword(string OldPassword)
        {
            UpdatePasswordUserRequestModel requestModel = new UpdatePasswordUserRequestModel()
            {
                OldPassword = OldPassword
            };

            _validator.ShouldHaveValidationErrorFor(r => r.OldPassword, requestModel).WithErrorMessage("old password must be greater than 8 characters.");
        }

        [Theory]
        [InlineData("greather than 60 characters greather than 60 characters greather than 60 characters greather than 60 characters greather than 60 characters ")]
        public void ShouldHaveValidationErrorWithGreaterThan60CharactersOldPassword(string OldPassword)
        {
            UpdatePasswordUserRequestModel requestModel = new UpdatePasswordUserRequestModel()
            {
                OldPassword = OldPassword
            };

            _validator.ShouldHaveValidationErrorFor(r => r.OldPassword, requestModel).WithErrorMessage("old password must be less than 60 characters.");
        }

        [Theory]
        [InlineData(null)]
        public void ShouldHaveValidationErrorWithNullOldPassword(string OldPassword)
        {
            UpdatePasswordUserRequestModel requestModel = new UpdatePasswordUserRequestModel()
            {
                OldPassword = OldPassword
            };

            _validator.ShouldHaveValidationErrorFor(r => r.OldPassword, requestModel).WithErrorMessage("old password cannot be null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("  ")]
        public void ShouldHaveValidationErrorWithEmptyOldPassword(string OldPassword)
        {
            UpdatePasswordUserRequestModel requestModel = new UpdatePasswordUserRequestModel()
            {
                OldPassword = OldPassword
            };

            _validator.ShouldHaveValidationErrorFor(r => r.OldPassword, requestModel).WithErrorMessage("old password cannot be empty.");
        }

        [Theory]
        [InlineData("password", "notEqualPassword")]
        public void ShouldHaveValidationErrorWithNotEqualNewPassword(string newPassword, string newPasswordConfirmation)
        {
            UpdatePasswordUserRequestModel requestModel = new UpdatePasswordUserRequestModel()
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
            UpdatePasswordUserRequestModel requestModel = new UpdatePasswordUserRequestModel()
            {
                NewPassword = newPassword,
                NewPasswordConfirmation = newPasswordConfirmation
            };

            _validator.ShouldNotHaveValidationErrorFor(r => r.NewPasswordConfirmation, requestModel);
        }
    }
}
