using System;
using ReviewApi.Domain.Entities;
using Xunit;

namespace ReviewApi.UnitTests.Domain.Entities
{
    public class ReviewImageTest
    {
        [Fact]
        public void ShouldConstructWithParameterlessConstructor()
        {
            ReviewImage reviewImage = new ReviewImage();

            Assert.Null(reviewImage.Review);
            Assert.Null(reviewImage.FileName);
            Assert.Null(reviewImage.FilePath);
            Assert.Equal(Guid.Empty, reviewImage.Id);
            Assert.Equal(Guid.Empty, reviewImage.ReviewId);
        }

        [Fact]
        public void ShouldConstructWithFilenameFilepathConstructor()
        {
            string filename = "FILENAME";
            string filepath = "FILEPATH";
            ReviewImage reviewImage = new ReviewImage(filename, filepath);

            Assert.Null(reviewImage.Review);
            Assert.Equal(filename, reviewImage.FileName);
            Assert.Equal(filepath, reviewImage.FilePath);
            Assert.Equal(Guid.Empty, reviewImage.Id);
            Assert.Equal(Guid.Empty, reviewImage.ReviewId);
        }

        [Fact]
        public void ShouldUpdate()
        {
            string filename = "FILENAME";
            string filepath = "FILEPATH";
            ReviewImage reviewImage = new ReviewImage();

            reviewImage.Update(filename, filepath);

            Assert.Null(reviewImage.Review);
            Assert.Equal(filename, reviewImage.FileName);
            Assert.Equal(filepath, reviewImage.FilePath);
            Assert.Equal(Guid.Empty, reviewImage.Id);
            Assert.Equal(Guid.Empty, reviewImage.ReviewId);
        }
    }
}
