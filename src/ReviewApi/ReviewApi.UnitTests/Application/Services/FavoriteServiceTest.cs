using System;
using System.Threading.Tasks;
using NSubstitute;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Services;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Exceptions;
using ReviewApi.Domain.Interfaces.Repositories;
using ReviewApi.Shared.Interfaces;
using Xunit;

namespace ReviewApi.UnitTests.Application.Services
{
    public class FavoriteServiceTest
    {
        private readonly IFavoriteRepository _favoriteRepositoryMock;
        private readonly IReviewRepository _reviewRepositoryMock;
        private readonly IUserRepository _userRepositoryMock;
        private readonly IFileUploadUtils _fileUploadUtils;
        private readonly IFavoriteService _favoriteService;

        public FavoriteServiceTest()
        {
            _favoriteRepositoryMock = NSubstitute.Substitute.For<IFavoriteRepository>();
            _reviewRepositoryMock = NSubstitute.Substitute.For<IReviewRepository>();
            _userRepositoryMock = NSubstitute.Substitute.For<IUserRepository>();
            _fileUploadUtils = NSubstitute.Substitute.For<IFileUploadUtils>();

            _favoriteService = new FavoriteService(_favoriteRepositoryMock, _reviewRepositoryMock, _userRepositoryMock, _fileUploadUtils, "");
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundExceptionOnCreateFavoriteInNotExistsReview()
        {
            _reviewRepositoryMock.AlreadyExists(Arg.Any<Guid>()).Returns(false);

            Exception exception = await Record.ExceptionAsync(() => _favoriteService.CreateOrDelete(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            Assert.IsType<ResourceNotFoundException>(exception);
            Assert.Equal("review not found.", exception.Message);
        }

        [Fact]
        public async Task ShouldCreateFavorite()
        {
            _reviewRepositoryMock.AlreadyExists(Arg.Any<Guid>()).Returns(true);
            _favoriteRepositoryMock.GetByUserIdAndReviewId(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(null as Favorite);

            Exception exception = await Record.ExceptionAsync(() => _favoriteService.CreateOrDelete(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            Assert.Null(exception);
            await _favoriteRepositoryMock.Received(1).Create(Arg.Any<Favorite>());
            await _favoriteRepositoryMock.Received(1).Save();
        }

        [Fact]
        public async Task ShouldDeleteFavorite()
        {
            _reviewRepositoryMock.AlreadyExists(Arg.Any<Guid>()).Returns(true);
            _favoriteRepositoryMock.GetByUserIdAndReviewId(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(new Favorite());

            Exception exception = await Record.ExceptionAsync(() => _favoriteService.CreateOrDelete(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            Assert.Null(exception);
            _favoriteRepositoryMock.Received(1).Delete(Arg.Any<Favorite>());
            await _favoriteRepositoryMock.Received(1).Save();
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundExceptionOnNotFindUser()
        {
            _userRepositoryMock.GetByName(Arg.Any<string>()).Returns(null as User);

            Exception exception = await Record.ExceptionAsync(() => _favoriteService.GetAllFromUser("username", 0, 0));

            Assert.IsType<ResourceNotFoundException>(exception);
        }
    }
}
