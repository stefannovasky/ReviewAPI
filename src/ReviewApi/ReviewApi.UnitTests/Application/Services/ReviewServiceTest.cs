using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NSubstitute;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models.Review;
using ReviewApi.Application.Services;
using ReviewApi.Domain.Dto;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Exceptions;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Infra.Redis.Interfaces;
using ReviewApi.Shared.Interfaces;
using Xunit;

namespace ReviewApi.UnitTests.Application.Services
{
    public class ReviewServiceTest
    {
        private readonly IReviewRepository _reviewRepositoryMock;
        private readonly IFileUploadUtils _fileUploadUtilsMock;
        private readonly ICacheDatabase _cacheDatabaseUtilsMock;
        private readonly IJsonUtils _jsonUtilsMock;
        private readonly IReviewService _reviewService;
        private readonly User _fakeInsertedConfirmedUser;
        private readonly User _fakeInsertedNotConfirmedUser;

        public ReviewServiceTest()
        {
            _reviewRepositoryMock = NSubstitute.Substitute.For<IReviewRepository>();
            _fileUploadUtilsMock = NSubstitute.Substitute.For<IFileUploadUtils>();
            _cacheDatabaseUtilsMock = NSubstitute.Substitute.For<ICacheDatabase>();
            _jsonUtilsMock = NSubstitute.Substitute.For<IJsonUtils>();
            _reviewService = new ReviewService(_reviewRepositoryMock, _fileUploadUtilsMock, _cacheDatabaseUtilsMock, _jsonUtilsMock, "webappurl");

            _fakeInsertedNotConfirmedUser = new User(Guid.NewGuid(), "user name", "user@mail.com", "password");
            _fakeInsertedConfirmedUser = new User(Guid.NewGuid(), "user name", "user@mail.com", "password");
            _fakeInsertedConfirmedUser.Confirm();
        }

        [Fact]
        public async Task ShouldCreateReview()
        {
            CreateOrUpdateReviewRequestModel model = new CreateOrUpdateReviewRequestModel()
            {
                Image = new MemoryStream(),
                Text = "TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT ",
                Title = "TITLE",
                Stars = 1
            };
            _fileUploadUtilsMock.UploadImage(Arg.Any<Stream>()).Returns(new FileDTO() { FileName = "FILENAME", FilePath = "FILEPATH" });

            Exception exception = await Record.ExceptionAsync(() => _reviewService.Create(Guid.NewGuid().ToString(), model));

            Assert.Null(exception);
            await _reviewRepositoryMock.Received(1).Create(Arg.Any<Review>());
            await _reviewRepositoryMock.Received(1).Save();
        }

        [Fact]
        public async Task ShouldGetAllReviews()
        {
            Review review1 = new Review("TITLE", "TEXT", 5, Guid.NewGuid());
            Review review2 = new Review("TITLE", "TEXT", 5, Guid.NewGuid());
            Review review3 = new Review("TITLE", "TEXT", 5, Guid.NewGuid());
            _reviewRepositoryMock.GetAll(Arg.Any<int>()).Returns(new List<Review>() { review1, review2, review3 });

            Exception exception = await Record.ExceptionAsync(() => _reviewService.GetAll());

            Assert.Null(exception);
        }

        [Fact]
        public async Task ShouldThrowReviewResourceNotFoundExceptionOnUpdate()
        {
            _reviewRepositoryMock.GetById(Arg.Any<Guid>()).Returns(null as Review);
            CreateOrUpdateReviewRequestModel model = new CreateOrUpdateReviewRequestModel()
            {
                Image = new MemoryStream(),
                Stars = 1,
                Text = "TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT",
                Title = "TITLE"
            };

            Exception exception = await Record.ExceptionAsync(() => _reviewService.Update(_fakeInsertedConfirmedUser.Id.ToString(), Guid.NewGuid().ToString(), model));

            Assert.IsType<ResourceNotFoundException>(exception);
        }

        [Fact]
        public async Task ShouldThrowForbiddenExceptionOnUpdate()
        {
            Guid reviewCreatorIdDifferentThanAuthenticatedUserId = Guid.NewGuid();
            _reviewRepositoryMock.GetById(Arg.Any<Guid>()).Returns(new Review("", "", 1, reviewCreatorIdDifferentThanAuthenticatedUserId));
            CreateOrUpdateReviewRequestModel model = new CreateOrUpdateReviewRequestModel()
            {
                Image = new MemoryStream(),
                Stars = 1,
                Text = "TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT",
                Title = "TITLE"
            };

            Exception exception = await Record.ExceptionAsync(() => _reviewService.Update(_fakeInsertedConfirmedUser.Id.ToString(), Guid.NewGuid().ToString(), model));

            Assert.IsType<ForbiddenException>(exception);
        }

        [Fact]
        public async Task ShouldThrowForbiddenExceptionODelete()
        {
            Guid reviewCreatorIdDifferentThanAuthenticatedUserId = Guid.NewGuid();
            _reviewRepositoryMock.GetById(Arg.Any<Guid>()).Returns(new Review("", "", 1, reviewCreatorIdDifferentThanAuthenticatedUserId));

            Exception exception = await Record.ExceptionAsync(() => _reviewService.Delete(_fakeInsertedConfirmedUser.Id.ToString(), Guid.NewGuid().ToString()));

            Assert.IsType<ForbiddenException>(exception);
        }

        [Fact]
        public async Task ShouldDelete()
        {
            Guid userId = Guid.NewGuid();
            _reviewRepositoryMock.GetById(Arg.Any<Guid>()).Returns(new Review("", "", 1, userId));

            Exception exception = await Record.ExceptionAsync(() => _reviewService.Delete(userId.ToString(), Guid.NewGuid().ToString()));

            Assert.Null(exception);
            _reviewRepositoryMock.Received(1).Delete(Arg.Any<Review>());
            await _reviewRepositoryMock.Received(1).Save();
            await _cacheDatabaseUtilsMock.Received(1).Remove(Arg.Any<string>());
        }

        [Fact]
        public async Task ShouldUpdate()
        {
            Guid reviewId = Guid.NewGuid();
            Review insertedReview = new Review(reviewId, "", "", 1, _fakeInsertedConfirmedUser.Id);
            _reviewRepositoryMock.GetById(Arg.Any<Guid>()).Returns(insertedReview);
            CreateOrUpdateReviewRequestModel model = new CreateOrUpdateReviewRequestModel()
            {
                Image = new MemoryStream(),
                Stars = 1,
                Text = "TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT",
                Title = "TITLE"
            };

            Exception exception = await Record.ExceptionAsync(() => _reviewService.Update(_fakeInsertedConfirmedUser.Id.ToString(), reviewId.ToString(), model));

            Assert.Null(exception);
            _reviewRepositoryMock.Received(1).Update(Arg.Any<Review>());
            await _reviewRepositoryMock.Received(1).Save();
            await _cacheDatabaseUtilsMock.Received(1).Set(Arg.Any<string>(), Arg.Any<string>());
            _jsonUtilsMock.Received(1).Serialize(Arg.Any<object>());
        }
    }
}
