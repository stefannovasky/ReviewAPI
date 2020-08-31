using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using ReviewApi.Domain.Entities;
using ReviewApi.Infra.Context;
using ReviewApi.IntegrationTests.CustomWebApplicationFactory;
using ReviewApi.IntegrationTests.Extensions;
using ReviewApi.IntegrationTests.Helpers;
using ReviewApi.Shared.Utils;
using Xunit;

namespace ReviewApi.IntegrationTests.Controllers
{
    [Collection("Sequential")]
    public class FavoriteControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _webApplicationFactory;
        private readonly HttpClient _httpClient;
        private readonly MainContext _database;
        private readonly CreateRequestHelper _createRequestHelper;
        private readonly AuthorizationTokenHelper _authorizationTokenHelper;
        private readonly TestFileHelper _fileHelper;
        private readonly User _insertedUser;

        public FavoriteControllerTest(CustomWebApplicationFactory<Startup> webApplicationFactory)
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
            _insertedUser = new User("User Name", "user@mail.com", new HashUtils().GenerateHash("User password"));
        }

        private async Task<User> InsertUserOnDatabase()
        {
            _database.Database.EnsureCreated();
            ProfileImage image = new ProfileImage("FILENAME", "FILEPATH");
            _insertedUser.AddProfileImage(image);
            await _database.Set<User>().AddAsync(_insertedUser);
            _insertedUser.Confirm();
            await _database.SaveChangesAsync();
            return _insertedUser;
        }

        private async Task<Review> InsertReviewOnDatabase()
        {
            _database.Database.EnsureCreated();
            ReviewImage image = new ReviewImage("FILENAME", "FILEPATH");
            Review review = new Review("TITLE", "TEXT", 1, _insertedUser.Id);
            review.AddImage(image);
            await _database.Set<Review>().AddAsync(review);
            await _database.SaveChangesAsync();
            return review;
        }

                private async Task<Favorite> InsertFavoriteOnDatabase(Guid userId, Guid reviewId)
        {
            Favorite favorite = new Favorite(userId, reviewId);

            _database.Database.EnsureCreated();
            await _database.Set<Favorite>().AddAsync(favorite);
            await _database.SaveChangesAsync();
            
            return favorite;
        }

        [Fact]
        public async Task ShouldReturnUnauthorizedOnCallCreate() 
        {
            HttpResponseMessage response = await _httpClient.PostAsync($"../reviews/{Guid.NewGuid().ToString()}/favorites", null);

            Assert.Equal((int)HttpStatusCode.Unauthorized, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnBadRequestOnCallCreateInNotExistsReview() 
        {
            User user = await InsertUserOnDatabase();
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(user.Id));

            HttpResponseMessage response = await _httpClient.PostAsync($"../reviews/{Guid.NewGuid().ToString()}/favorites", null);

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnBadRequestOnCallCreateAlreadyExistsFavorite()
        {
            User user = await InsertUserOnDatabase();
            Review review = await InsertReviewOnDatabase();
            Favorite favorite = await InsertFavoriteOnDatabase(user.Id, review.Id);
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(user.Id));

            HttpResponseMessage response = await _httpClient.PostAsync($"../reviews/{review.Id.ToString()}/favorites", null);

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnOkOnCallCreate()
        {
            User user = await InsertUserOnDatabase();
            Review review = await InsertReviewOnDatabase();
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(user.Id));

            HttpResponseMessage response = await _httpClient.PostAsync($"../reviews/{review.Id.ToString()}/favorites", null);

            Assert.Equal((int)HttpStatusCode.OK, (int)response.StatusCode);
        }
    }
}
