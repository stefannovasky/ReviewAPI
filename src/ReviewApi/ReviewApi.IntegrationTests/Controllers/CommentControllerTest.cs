using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using ReviewApi.Application.Models.Comment;
using ReviewApi.Domain.Entities;
using ReviewApi.Infra.Context;
using ReviewApi.IntegrationTests.CustomWebApplicationFactory;
using ReviewApi.IntegrationTests.Extensions;
using ReviewApi.IntegrationTests.Helpers;
using Xunit;

namespace ReviewApi.IntegrationTests.Controllers
{
    [Collection("Sequential")]
    public class CommentControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _webApplicationFactory;
        private readonly HttpClient _httpClient;
        private readonly MainContext _database;
        private readonly CreateRequestHelper _createRequestHelper;
        private readonly AuthorizationTokenHelper _authorizationTokenHelper;
        private readonly TestFileHelper _fileHelper;

        public CommentControllerTest(CustomWebApplicationFactory<Startup> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
            _httpClient = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions());
            _createRequestHelper = new CreateRequestHelper();
            _authorizationTokenHelper = new AuthorizationTokenHelper();
            _fileHelper = new TestFileHelper();

            DbContextOptions<MainContext> options = new DbContextOptionsBuilder<MainContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _database = new MainContext(options);
        }

        private async Task<User> InsertUserOnDatabase()
        {
            User user = new User("name", "email", "password");
            _database.Database.EnsureCreated();
            ProfileImage image = new ProfileImage("FILENAME", "FILEPATH");
            user.AddProfileImage(image);
            await _database.Set<User>().AddAsync(user);
            user.Confirm();
            await _database.SaveChangesAsync();
            return user;
        }

        private async Task<Review> InsertReviewOnDatabase(Guid userId)
        {
            _database.Database.EnsureCreated();
            ReviewImage image = new ReviewImage("FILENAME", "FILEPATH");
            Review review = new Review("TITLE", "TEXT", 1, userId);
            review.AddImage(image);
            await _database.Set<Review>().AddAsync(review);
            await _database.SaveChangesAsync();
            return review;
        }

        private async Task<Comment> InsertCommentOnDatabase(Guid reviewId, Guid userId)
        {
            _database.Database.EnsureCreated();
            Comment comment = new Comment("TEXT", userId, reviewId);
            await _database.Set<Comment>().AddAsync(comment);
            await _database.SaveChangesAsync();
            return comment;
        }

        [Fact]
        public async Task ShouldThrowUnauthorizedOnCallCreate() 
        {
            HttpResponseMessage response = await _httpClient.PostAsync($"../reviews/{Guid.NewGuid().ToString()}/comments", _createRequestHelper.CreateStringContent(new { }));

            Assert.Equal((int)HttpStatusCode.Unauthorized, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldThrowUnauthorizedOnCallGetAllFromReview()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"../reviews/{Guid.NewGuid().ToString()}/comments");

            Assert.Equal((int)HttpStatusCode.Unauthorized, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldThrowUnauthorizedOnCallGetById()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"../reviews/{Guid.NewGuid().ToString()}/comments/{Guid.NewGuid().ToString()}");

            Assert.Equal((int)HttpStatusCode.Unauthorized, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldThrowUnauthorizedOnCallUpdate()
        {
            HttpResponseMessage response = await _httpClient.PutAsync($"../reviews/{Guid.NewGuid().ToString()}/comments/{Guid.NewGuid().ToString()}", _createRequestHelper.CreateStringContent(new { }));

            Assert.Equal((int)HttpStatusCode.Unauthorized, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldThrowUnauthorizedOnCallDelete()
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"../reviews/{Guid.NewGuid().ToString()}/comments/{Guid.NewGuid().ToString()}");

            Assert.Equal((int)HttpStatusCode.Unauthorized, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldCreateComment() 
        {
            User insertedUser = await InsertUserOnDatabase();
            Review insertedReview = await InsertReviewOnDatabase(insertedUser.Id);
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(insertedUser.Id));

            CreateCommentRequestModel requestModel = new CreateCommentRequestModel()
            {
                Text = "TEXT"
            };
            HttpResponseMessage response = await _httpClient.PostAsync($"../reviews/{insertedReview.Id.ToString()}/comments", _createRequestHelper.CreateStringContent(requestModel));

            Assert.Equal((int)HttpStatusCode.Created, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldUpdateComment()
        {
            User insertedUser = await InsertUserOnDatabase();
            Review insertedReview = await InsertReviewOnDatabase(insertedUser.Id);
            Comment insertedComment = await InsertCommentOnDatabase(insertedReview.Id, insertedUser.Id);
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(insertedUser.Id));

            CreateCommentRequestModel requestModel = new CreateCommentRequestModel()
            {
                Text = "TEXT"
            };
            HttpResponseMessage response = await _httpClient.PutAsync($"../reviews/{insertedReview.Id.ToString()}/comments/{insertedComment.Id.ToString()}", _createRequestHelper.CreateStringContent(requestModel));

            Assert.Equal((int)HttpStatusCode.OK, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldDeleteComment()
        {
            User insertedUser = await InsertUserOnDatabase();
            Review insertedReview = await InsertReviewOnDatabase(insertedUser.Id);
            Comment insertedComment = await InsertCommentOnDatabase(insertedReview.Id, insertedUser.Id);
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(insertedUser.Id));

            CreateCommentRequestModel requestModel = new CreateCommentRequestModel()
            {
                Text = "TEXT"
            };
            HttpResponseMessage response = await _httpClient.DeleteAsync($"../reviews/{insertedReview.Id.ToString()}/comments/{insertedComment.Id.ToString()}");

            Assert.Equal((int)HttpStatusCode.NoContent, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldGetAllComments() 
        {
            User insertedUser = await InsertUserOnDatabase();
            Review insertedReview = await InsertReviewOnDatabase(insertedUser.Id);
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(insertedUser.Id));

            CreateCommentRequestModel requestModel = new CreateCommentRequestModel()
            {
                Text = "TEXT"
            };
            HttpResponseMessage response = await _httpClient.GetAsync($"../reviews/{insertedReview.Id.ToString()}/comments");

            Assert.Equal((int)HttpStatusCode.OK, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldGetCommentById()
        {
            User insertedUser = await InsertUserOnDatabase();
            Review insertedReview = await InsertReviewOnDatabase(insertedUser.Id);
            Comment insertedComment = await InsertCommentOnDatabase(insertedReview.Id, insertedUser.Id);
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(insertedUser.Id));

            CreateCommentRequestModel requestModel = new CreateCommentRequestModel()
            {
                Text = "TEXT"
            };
            HttpResponseMessage response = await _httpClient.GetAsync($"../reviews/{insertedReview.Id.ToString()}/comments/{insertedComment.Id.ToString()}");

            Assert.Equal((int)HttpStatusCode.OK, (int)response.StatusCode);
        }
    }

}
