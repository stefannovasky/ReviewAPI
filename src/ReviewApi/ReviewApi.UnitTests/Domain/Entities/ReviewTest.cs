using System;
using ReviewApi.Domain.Entities;
using Xunit;

namespace ReviewApi.UnitTests.Domain.Entities
{
    public class ReviewTest
    {
        [Fact]
        public void ShouldConstructWithParameterlessConstructor()
        {
            Review review = new Review();

            Assert.Equal(Guid.Empty, review.Id);
            Assert.Equal(Guid.Empty, review.CreatorId);
            Assert.Equal(0, review.Stars);
            Assert.Null(review.Creator);
            Assert.Null(review.Image);
            Assert.Null(review.Text);
            Assert.Null(review.Title);
        }

        [Fact]
        public void ShouldConstructWithAllParametersConstructor()
        {
            string title = "TITLE";
            string text = "TEXT";
            int stars = 1;
            Guid creatorId = Guid.NewGuid(); 
            Review review = new Review(title, text, stars, creatorId);

            Assert.Equal(title, review.Title);
            Assert.Equal(text, review.Text);
            Assert.Equal(stars, review.Stars);
            Assert.Equal(creatorId, review.CreatorId);
        }

        [Fact]
        public void ShouldAddImage()
        {
            Review review = new Review();
            ReviewImage image = new ReviewImage("FILENAME", "FILEPATH");

            review.AddImage(image);

            Assert.Equal(image.FileName, review.Image.FileName);
            Assert.Equal(image.FilePath, review.Image.FilePath);
        }
    }
}
