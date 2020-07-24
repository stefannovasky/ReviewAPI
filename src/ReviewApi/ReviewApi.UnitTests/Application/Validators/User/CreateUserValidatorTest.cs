using FluentValidation.TestHelper;
using ReviewApi.Application.Models.User;
using ReviewApi.Application.Validators;
using Xunit;

namespace ReviewApi.UnitTests.Application.Validators.User
{
    public class CreateUserValidatorTest
    {
        private readonly CreateUserValidator _validator;

        public CreateUserValidatorTest()
        {
            _validator = new CreateUserValidator(); 
        }

        [Theory]
        [InlineData(null)]
        public void ShouldHaveValidationErrorWithNullEmail(string email) 
        {
            CreateUserRequestModel requestModel = new CreateUserRequestModel()
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
            CreateUserRequestModel requestModel = new CreateUserRequestModel()
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
            CreateUserRequestModel requestModel = new CreateUserRequestModel()
            {
                Email = email
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Email, requestModel).WithErrorMessage("email must be a valid email.");
        }


        [Theory]
        [InlineData("email.greater.than.244.chars.email.greater.than.244.chars.email.greater.than.244.chars.email.greater.than.244.charsemail.greater.than.244.chars.email.greater.than.244.chars.email.greater.than.244.chars..email.greater.than.244.chars..email.greater.than.244.chars@mail.com")]
        public void ShouldHaveValidationErrorGreaterThan255CharactersEmail(string email)
        {
            CreateUserRequestModel requestModel = new CreateUserRequestModel()
            {
                Email = email
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Email, requestModel).WithErrorMessage("email must be less than 254 characters.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ShouldHaveValidationErrorWithEmptyName(string name)
        {
            CreateUserRequestModel requestModel = new CreateUserRequestModel()
            {
                Name = name
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Name, requestModel).WithErrorMessage("name cannot be empty.");
        }

        [Theory]
        [InlineData(null)]
        public void ShouldHaveValidationErrorWithNullName(string name)
        {
            CreateUserRequestModel requestModel = new CreateUserRequestModel()
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
            CreateUserRequestModel requestModel = new CreateUserRequestModel()
            {
                Name = name
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Name, requestModel).WithErrorMessage("name must be greater than 2 characters.");
        }

        [Theory]
        [InlineData("greather than 150 characters. greather than 150 characters. greather than 150 characters. greather than 150 characters. greather than 150 characters. greather than 150 characters. greather than 150 characters. greather than 150 characters. greather than 150 characters. ")]
        public void ShouldHaveValidationErrorWithGreatherThan150CharactersName(string name)
        {
            CreateUserRequestModel requestModel = new CreateUserRequestModel()
            {
                Name = name
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Name, requestModel).WithErrorMessage("name must be less than 150 characters.");
        }

        [Theory]
        [InlineData("aaaaaaa")]
        [InlineData("aa")]
        [InlineData("a")]
        public void ShouldHaveValidationErrorWithLessThan8CharactersPassword(string password)
        {
            CreateUserRequestModel requestModel = new CreateUserRequestModel()
            {
                Password = password
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Password, requestModel).WithErrorMessage("password must be greater than 8 characters.");
        }

        [Theory]
        [InlineData("greather than 60 characters greather than 60 characters greather than 60 characters greather than 60 characters greather than 60 characters ")]
        public void ShouldHaveValidationErrorWithGreaterThan60CharactersPassword(string password)
        {
            CreateUserRequestModel requestModel = new CreateUserRequestModel()
            {
                Password = password
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Password, requestModel).WithErrorMessage("password must be less than 60 characters.");
        }

        [Theory]
        [InlineData(null)]
        public void ShouldHaveValidationErrorWithNullPassword(string password)
        {
            CreateUserRequestModel requestModel = new CreateUserRequestModel()
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
            CreateUserRequestModel requestModel = new CreateUserRequestModel()
            {
                Password = password
            };

            _validator.ShouldHaveValidationErrorFor(r => r.Password, requestModel).WithErrorMessage("password cannot be empty.");
        }
    }
}
