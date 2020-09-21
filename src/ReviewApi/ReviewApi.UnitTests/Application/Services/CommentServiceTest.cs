using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute;
using ReviewApi.Application.Converter;
using ReviewApi.Application.Interfaces;
using ReviewApi.Application.Models.Comment;
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
    public class CommentServiceTest
    {
        private readonly ICommentRepository _commentRepositoryMock;
        private readonly IReviewRepository _reviewRepositoryMock;
        private readonly IConverter _converterMock;
        private readonly ICacheDatabase _cacheDatabaseMock;
        private readonly IJsonUtils _jsonUtilsMock;
        private readonly ICommentService _commentService;

        public CommentServiceTest()
        {
            _commentRepositoryMock = NSubstitute.Substitute.For<ICommentRepository>();
            _reviewRepositoryMock = NSubstitute.Substitute.For<IReviewRepository>();
            _cacheDatabaseMock = NSubstitute.Substitute.For<ICacheDatabase>();
            _jsonUtilsMock = NSubstitute.Substitute.For<IJsonUtils>();
            _converterMock = NSubstitute.Substitute.For<IConverter>();

            _commentService = new CommentService(_commentRepositoryMock, _reviewRepositoryMock, _cacheDatabaseMock, _jsonUtilsMock, _converterMock, "");
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundExceptionOnCreateCommentWithNotExistsReview() 
        {
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns(null as string);
            _reviewRepositoryMock.AlreadyExists(Arg.Any<Guid>()).Returns(false);
            CreateCommentRequestModel requestModel = new CreateCommentRequestModel()
            {
                Text = "TEXT"
            };

            Exception exception = await Record.ExceptionAsync(() => _commentService.Create(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), requestModel));

            Assert.IsType<ResourceNotFoundException>(exception);
        }

        [Fact]
        public async Task ShouldCreateComment() 
        {
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns(null as string);
            _reviewRepositoryMock.AlreadyExists(Arg.Any<Guid>()).Returns(true);
            CreateCommentRequestModel requestModel = new CreateCommentRequestModel()
            {
                Text = "TEXT"
            };

            Exception exception = await Record.ExceptionAsync(() => _commentService.Create(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), requestModel));

            Assert.Null(exception);
            await _commentRepositoryMock.Received(1).Create(Arg.Any<Comment>());
            await _commentRepositoryMock.Received(1).Save();
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundExceptionOnGetAllCommentsInNotExistsReview() 
        {
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns(null as string);
            _reviewRepositoryMock.AlreadyExists(Arg.Any<Guid>()).Returns(false);

            Exception exception = await Record.ExceptionAsync(() => _commentService.GetAllFromReview(Guid.NewGuid().ToString(), new PaginationDTO(1, 10)));

            Assert.IsType<ResourceNotFoundException>(exception);
        }

        [Fact]
        public async Task ShouldGetAllCommentsFromCache()
        {
            Comment[] insertedsComments = { new Comment("text", Guid.NewGuid(), Guid.NewGuid()) };
            _jsonUtilsMock.Deserialize<IEnumerable<Comment>>(Arg.Any<string>()).Returns(insertedsComments);
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns("comments");
            _reviewRepositoryMock.AlreadyExists(Arg.Any<Guid>()).Returns(true);
            _commentRepositoryMock.CountFromReview(Arg.Any<Guid>()).Returns(1);

            Exception exception = await Record.ExceptionAsync(() => _commentService.GetAllFromReview(Guid.NewGuid().ToString(), new PaginationDTO(1, 10)));

            Assert.Null(exception);
        }

        [Fact]
        public async Task ShouldGetAllCommentsFromDatabase()
        {
            Comment[] insertedsComments = { new Comment("text", Guid.NewGuid(), Guid.NewGuid()) };
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns(null as string);
            _reviewRepositoryMock.AlreadyExists(Arg.Any<Guid>()).Returns(true);
            _commentRepositoryMock.CountFromReview(Arg.Any<Guid>()).Returns(1);
            _commentRepositoryMock.GetAllFromReview(Arg.Any<Guid>(), Arg.Any<PaginationDTO>()).Returns(insertedsComments);

            Exception exception = await Record.ExceptionAsync(() => _commentService.GetAllFromReview(Guid.NewGuid().ToString(), new PaginationDTO(1, 10)));

            Assert.Null(exception);
            await _cacheDatabaseMock.Received(1).Set(Arg.Any<string>(), Arg.Any<string>());
            _jsonUtilsMock.Received(1).Serialize(Arg.Any<object>());
        }

        [Fact]
        public async Task ShouldDeleteCommentFromCache()
        {
            Guid userId = Guid.NewGuid();
            Comment insertedComment = new Comment("text", userId, Guid.NewGuid());
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns("comment");
            _jsonUtilsMock.Deserialize<Comment>(Arg.Any<string>()).Returns(insertedComment);

            Exception exception = await Record.ExceptionAsync(() => _commentService.Delete(Guid.NewGuid().ToString(), userId.ToString()));

            Assert.Null(exception);
            await _cacheDatabaseMock.Received(1).Remove(Arg.Any<string>());
            _commentRepositoryMock.Received(1).Delete(Arg.Any<Comment>());
            await _commentRepositoryMock.Received(1).Save();
        }

        [Fact]
        public async Task ShouldDeleteCommentFromDatabase() 
        {
            Guid userId = Guid.NewGuid();
            Comment insertedComment = new Comment("text", userId, Guid.NewGuid());
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns(null as string);
            _commentRepositoryMock.GetById(Arg.Any<Guid>()).Returns(insertedComment);

            Exception exception = await Record.ExceptionAsync(() => _commentService.Delete(Guid.NewGuid().ToString(), userId.ToString()));

            Assert.Null(exception);
            _commentRepositoryMock.Received(1).Delete(Arg.Any<Comment>());
            await _commentRepositoryMock.Received(1).Save();
        }

        [Fact]
        public async Task ShouldThrowForbiddenExceptionOnTryDeleteComment() 
        {
            Comment insertedComment = new Comment("text", Guid.NewGuid(), Guid.NewGuid());
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns(null as string);
            _commentRepositoryMock.GetById(Arg.Any<Guid>()).Returns(insertedComment);

            Exception exception = await Record.ExceptionAsync(() => _commentService.Delete(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            Assert.IsType<ForbiddenException>(exception);
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundExceptionWhenTryDeleteNotExistsComment() 
        {
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns(null as string);
            _commentRepositoryMock.GetById(Arg.Any<Guid>()).Returns(null as Comment);

            Exception exception = await Record.ExceptionAsync(() => _commentService.Delete(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));

            Assert.IsType<ResourceNotFoundException>(exception);
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundExceptionOnGetById() 
        {
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns(null as string);
            _commentRepositoryMock.GetByIdIncludingUser(Arg.Any<Guid>()).Returns(null as Comment);

            Exception exception = await Record.ExceptionAsync(() => _commentService.GetById(Guid.NewGuid().ToString()));

            Assert.IsType<ResourceNotFoundException>(exception);
        }

        [Fact]
        public async Task ShouldGetByIdFromCache()
        {
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns("");
            _jsonUtilsMock.Deserialize<Comment>(Arg.Any<string>()).Returns(new Comment());

            Exception exception = await Record.ExceptionAsync(() => _commentService.GetById(Guid.NewGuid().ToString()));

            Assert.Null(exception);
        }

        [Fact]
        public async Task ShouldGetByIdFromDatabase()
        {
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns(null as string);
            _commentRepositoryMock.GetByIdIncludingUser(Arg.Any<Guid>()).Returns(new Comment());

            Exception exception = await Record.ExceptionAsync(() => _commentService.GetById(Guid.NewGuid().ToString()));

            Assert.Null(exception);
        }

        [Fact]
        public async Task ShouldThrowResourceNotFoundExceptionOnUpdate() 
        {
            CreateCommentRequestModel requestModel = new CreateCommentRequestModel() { Text = "TEXT" };
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns(null as string);
            _commentRepositoryMock.GetById(Arg.Any<Guid>()).Returns(null as Comment);

            Exception exception = await Record.ExceptionAsync(() => _commentService.Update(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), requestModel));

            Assert.IsType<ResourceNotFoundException>(exception);
        }

        [Fact]
        public async Task ShouldThrowForbiddenExceptionOnUpdate() 
        {
            CreateCommentRequestModel requestModel = new CreateCommentRequestModel() { Text = "TEXT" };
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns(null as string);
            _commentRepositoryMock.GetByIdIncludingUser(Arg.Any<Guid>()).Returns(new Comment("TEXT", Guid.NewGuid(), Guid.NewGuid()));

            Exception exception = await Record.ExceptionAsync(() => _commentService.Update(Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), requestModel));

            Assert.IsType<ForbiddenException>(exception);
        }

        [Fact]
        public async Task ShouldUpdateCommentFromCache() 
        {
            Guid userId = Guid.NewGuid();
            CreateCommentRequestModel requestModel = new CreateCommentRequestModel() { Text = "TEXT" };
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns("comment");
            _jsonUtilsMock.Deserialize<Comment>(Arg.Any<string>()).Returns(new Comment("TEXT", userId, Guid.NewGuid()));

            Exception exception = await Record.ExceptionAsync(() => _commentService.Update(Guid.NewGuid().ToString(), userId.ToString(), requestModel));

            Assert.Null(exception);
            _commentRepositoryMock.Received(1).Update(Arg.Any<Comment>());
            await _commentRepositoryMock.Received(1).Save();
            await _cacheDatabaseMock.Received(1).Set(Arg.Any<string>(), Arg.Any<string>());
        }

        [Fact]
        public async Task ShouldUpdateCommentFromDatabase()
        {
            Guid userId = Guid.NewGuid();
            CreateCommentRequestModel requestModel = new CreateCommentRequestModel() { Text = "TEXT" };
            _cacheDatabaseMock.Get(Arg.Any<string>()).Returns(null as string);
            _commentRepositoryMock.GetByIdIncludingUser(Arg.Any<Guid>()).Returns(new Comment("TEXT", userId, Guid.NewGuid()));

            Exception exception = await Record.ExceptionAsync(() => _commentService.Update(Guid.NewGuid().ToString(), userId.ToString(), requestModel));

            Assert.Null(exception);
            _commentRepositoryMock.Received(1).Update(Arg.Any<Comment>());
            await _commentRepositoryMock.Received(1).Save();
            await _cacheDatabaseMock.Received(1).Set(Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
