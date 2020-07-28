using FluentValidation.TestHelper;
using ReviewApi.Application.Models.User;
using ReviewApi.Application.Validators.User;
using Xunit;

namespace ReviewApi.UnitTests.Application.Validators.User
{
    public class UpdateNameUserValidatorTest
    {
        private readonly UpdateNameUserValidator _validator;
        public UpdateNameUserValidatorTest()
        {
            _validator = new UpdateNameUserValidator(); 
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ShouldHaveValidationErrorWithEmptyName(string name)
        {
            UpdateNameUserRequestModel requestModel = new UpdateNameUserRequestModel()
            {
                Name = name
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Name, requestModel).WithErrorMessage("name cannot be empty.");
        }

        [Theory]
        [InlineData(null)]
        public void ShouldHaveValidationErrorWithNullName(string name)
        {
            UpdateNameUserRequestModel requestModel = new UpdateNameUserRequestModel()
            {
                Name = name
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Name, requestModel).WithErrorMessage("name cannot be empty.");
        }


        [Theory]
        [InlineData("aa")]
        [InlineData("a")]
        public void ShouldHaveValidationErrorWithLessThan3CharactersName(string name)
        {
            UpdateNameUserRequestModel requestModel = new UpdateNameUserRequestModel()
            {
                Name = name
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Name, requestModel).WithErrorMessage("name must be greater than 2 characters.");
        }

        [Theory]
        [InlineData("greather than 150 characters. greather than 150 characters. greather than 150 characters. greather than 150 characters. greather than 150 characters. greather than 150 characters. greather than 150 characters. greather than 150 characters. greather than 150 characters. ")]
        public void ShouldHaveValidationErrorWithGreatherThan150CharactersName(string name)
        {
            UpdateNameUserRequestModel requestModel = new UpdateNameUserRequestModel()
            {
                Name = name
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Name, requestModel).WithErrorMessage("name must be less than 150 characters.");
        }
    }
}
