using System;
using NSubstitute;
using ReviewApi.Application.Converter;
using ReviewApi.Application.Models.Review;
using ReviewApi.Application.Models.User;
using ReviewApi.Domain.Entities;
using ReviewApi.Shared.Interfaces;
using Xunit;

namespace ReviewApi.UnitTests.Application.Converter
{
    public class ConverterTest
    {
        private readonly IFileUploadUtils _fileUploadUtilsMock;
        private readonly IConverter _converter;

        public ConverterTest()
        {
            _fileUploadUtilsMock = NSubstitute.Substitute.For<IFileUploadUtils>();
            _converter = new ReviewApi.Application.Converter.Converter(_fileUploadUtilsMock); 
        }

        [Fact]
        public void ShouldConvertUserToUserProfileResponseModel() 
        {
            User user = new User(Guid.NewGuid(), "name", "user@mail.com", "password");
            ProfileImage profileImage = new ProfileImage("filename", "filepath");
            user.AddProfileImage(profileImage);
            _fileUploadUtilsMock.GenerateImageUrl(Arg.Any<string>()).Returns("imageurl");

            UserProfileResponseModel responseModel = _converter.ConvertUserToUserProfileResponseModel(user);

            Assert.Equal(user.Name, responseModel.Name);
            Assert.Equal(user.Email, responseModel.Email);
            Assert.Equal("imageurl", responseModel.Image);
        }

        [Fact]
        public void ShouldConvertReviewToReviewResponseModel() 
        {
            User user = new User(Guid.NewGuid(), "name", "user@mail.com", "password");
            Review review = new Review(Guid.NewGuid(), "title", "text", 5, Guid.NewGuid());
            ReviewImage reviewImage = new ReviewImage("filename", "filepath");
            review.AddImage(reviewImage);
            review.UpdateCreator(user);
            _fileUploadUtilsMock.GenerateImageUrl(Arg.Any<string>()).Returns("imageurl");

            ReviewResponseModel responseModel = _converter.ConvertReviewToReviewResponseModel(review);

            Assert.Equal(review.Title, responseModel.Title);
            Assert.Equal(review.Text, responseModel.Text);
            Assert.Equal(review.Stars, responseModel.Stars);
            Assert.Equal(review.Creator.Name, responseModel.Creator);
            Assert.Equal("imageurl", responseModel.Image);
        }
    }
}
