﻿using System;
using System.Threading.Tasks;
using NSubstitute;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Services;
using ReviewApi.Domain.Entities;
using ReviewApi.Domain.Exceptions;
using ReviewApi.Domain.Interfaces.Repositories;
using Xunit;

namespace ReviewApi.UnitTests.Application.Services
{
    public class FavoriteServiceTest
    {
        private readonly IFavoriteRepository _favoriteRepositoryMock;
        private readonly IReviewRepository _reviewRepositoryMock;
        private readonly IFavoriteService _favoriteService;

        public FavoriteServiceTest()
        {
            _favoriteRepositoryMock = NSubstitute.Substitute.For<IFavoriteRepository>();
            _reviewRepositoryMock = NSubstitute.Substitute.For<IReviewRepository>();

            _favoriteService = new FavoriteService(_favoriteRepositoryMock, _reviewRepositoryMock);
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundExceptionOnCreateFavoriteInNotExistsReview() 
        {
            _reviewRepositoryMock.AlreadyExists(Arg.Any<Guid>()).Returns(false);

            Exception exception = await Record.ExceptionAsync(() => _favoriteService.Create(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            Assert.IsType<ResourceNotFoundException>(exception);
            Assert.Equal("review not found.", exception.Message);
        }

        [Fact]
        public async Task ShouldThrowAlreadyExistsExceptionOnCreateAlreadyExistsFavorite()
        {
            _reviewRepositoryMock.AlreadyExists(Arg.Any<Guid>()).Returns(true);
            _favoriteRepositoryMock.AlreadyExists(Arg.Any<Guid>(), Arg.Any<Guid>()).Returns(true);

            Exception exception = await Record.ExceptionAsync(() => _favoriteService.Create(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            Assert.IsType<AlreadyExistsException>(exception);
            Assert.Equal("favorite already exists.", exception.Message);
        }
    }
}