using System.Threading.Tasks;
using FluentValidation.TestHelper;
using ReviewApi.Application.Validators.Comment;
using Xunit;

namespace ReviewApi.UnitTests.Application.Validators.Comment
{
    public class CreateOrUpdateCommentValidatorTest
    {
        private readonly CreateOrUpdateCommentValidator _validator; 

        public CreateOrUpdateCommentValidatorTest()
        {
            _validator = new CreateOrUpdateCommentValidator(); 
        }

        [Theory]
        [InlineData(null)]
        public void ShouldHaveErrorIfTextPropertyIsNull(string text)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Text, text).WithErrorMessage("comment text cannot be null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("                ")]
        public void ShouldHaveErrorIfTextPropertyIsEmpty(string text)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Text, text).WithErrorMessage("comment text cannot be empty.");
        }


        [Fact]
        public void ShouldHaveErrorIfTextPropertyIsGreaterThan1500Characters()
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Text, new string('a', 1501)).WithErrorMessage("comment text must be less than 1501 characters.");
        }
    }
}
