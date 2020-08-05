using FluentValidation.TestHelper;
using ReviewApi.Application.Validators.Review;
using Xunit;

namespace ReviewApi.UnitTests.Application.Validators.Review
{
    public class CreateOrUpdateReviewValidatorTest
    {
        private readonly CreateOrUpdateReviewValidator _validator;

        public CreateOrUpdateReviewValidatorTest()
        {
            _validator = new CreateOrUpdateReviewValidator();
        }

        [Theory]
        [InlineData(null)]
        public void ShouldHaveValidationErrorWithNullTitle(string title)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Title, title).WithErrorMessage("review title cannot be null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        public void ShouldHaveValidationErrorWithEmptyTitle(string title)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Title, title).WithErrorMessage("review title cannot be empty.");
        }

        [Theory]
        [InlineData("1")]
        public void ShouldHaveValidationErrorWithLessThan2CharactersTitle(string title)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Title, title).WithErrorMessage("review title must be greater than 1 characters.");
        }

        [Fact]
        public void ShouldHaveValidationErrorWithGreaterThan150CharactersTitle()
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Title, new string('a', 151)).WithErrorMessage("review title must be less than 151 characters.");
        }

        [Theory]
        [InlineData(null)]
        public void ShouldHaveValidationErrorWithNullText(string text)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Text, text).WithErrorMessage("review text cannot be null.");
        }

        [Theory]
        [InlineData("")]
        [InlineData("    ")]
        public void ShouldHaveValidationErrorWithEmptyText(string text)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Text, text).WithErrorMessage("review text cannot be empty.");
        }

        [Theory]
        [InlineData("1")]
        public void ShouldHaveValidationErrorWithLessThan20CharactersText(string text)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Text, text).WithErrorMessage("review text must be greater than 20 characters.");
        }

        [Fact]
        public void ShouldHaveValidationErrorWithGreaterThan1500CharactersText()
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Text, new string('a', 1501)).WithErrorMessage("review text must be less than 1501 characters.");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(6)]
        public void ShouldHaveValidationErrorWithInvalidStars(int stars)
        {
            _validator.ShouldHaveValidationErrorFor(p => p.Stars, stars);
        }
    }
}
