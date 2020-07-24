using FluentValidation.TestHelper;
using ReviewApi.Application.Models.User;
using ReviewApi.Application.Validators;
using Xunit;

namespace ReviewApi.UnitTests.Application.Validators.User
{
    public class ConfirmUserValidatorTest
    {
        private readonly ConfirmUserValidator _validator;

        public ConfirmUserValidatorTest()
        {
            _validator = new ConfirmUserValidator();
        }

        [Theory]
        [InlineData(null)]
        public void ShouldHaveValidationErrorWithNullCode(string code)
        {
            ConfirmUserRequestModel requestModel = new ConfirmUserRequestModel() { Code = code };

            _validator.ShouldHaveValidationErrorFor(p => p.Code, requestModel).WithErrorMessage("code cannot be null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ShouldHaveValidationErrorWithEmptyCode(string code)
        {
            ConfirmUserRequestModel requestModel = new ConfirmUserRequestModel() { Code = code };

            _validator.ShouldHaveValidationErrorFor(p => p.Code, requestModel).WithErrorMessage("code cannot be empty.");
        }

        [Theory]
        [InlineData("1")]
        [InlineData("1234567")]
        [InlineData("123456789")]
        public void ShouldHaveValidationErrorWithInvalidLengthCode(string code)
        {
            ConfirmUserRequestModel requestModel = new ConfirmUserRequestModel() { Code = code };

            _validator.ShouldHaveValidationErrorFor(p => p.Code, requestModel).WithErrorMessage("code must be 8 characters.");
        }
    }
}
