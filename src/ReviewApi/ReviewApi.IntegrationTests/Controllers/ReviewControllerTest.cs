using System;
using System.IO;
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
    public class ReviewControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _webApplicationFactory;
        private readonly HttpClient _httpClient;
        private readonly MainContext _database;
        private readonly CreateRequestHelper _createRequestHelper;
        private readonly AuthorizationTokenHelper _authorizationTokenHelper;
        private readonly User _insertedUser;

        public ReviewControllerTest(CustomWebApplicationFactory<Startup> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
            _httpClient = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions());
            _createRequestHelper = new CreateRequestHelper();
            _authorizationTokenHelper = new AuthorizationTokenHelper();

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

        private MultipartFormDataContent CreateReviewMultipartFormContent()
        {
            byte[] imageFile = File.ReadAllBytes(@"D:\images\83517838_1112176775819961_5680193972090330216_n.jpg");

            MultipartFormDataContent form = new MultipartFormDataContent()
            {
                { new StringContent("TITLE"), "title" },
                { new StringContent("TEXT TEXT TEXT TEXT TEXT TEXT TEXT TEXT "), "text" },
                { new StringContent("1"), "stars" },
            };
            form.Add(new ByteArrayContent(imageFile), "image", "imagefilename.jpg");
            return form;
        }

        [Fact]
        public async Task ShouldReturnCreatedAtStatusCodeOnCallCreateReview()
        {
            User user = await InsertUserOnDatabase();
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(user.Id));

            HttpResponseMessage response = await _httpClient.PostAsync("../reviews", CreateReviewMultipartFormContent());

            Assert.Equal((int)HttpStatusCode.Created, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnUnauthorizedStatusCodeOnCallCreateReview()
        {
            HttpResponseMessage response = await _httpClient.PostAsync("../reviews", CreateReviewMultipartFormContent());

            Assert.Equal((int)HttpStatusCode.Unauthorized, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnOkStatusCodeOnCallGetAll()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("../reviews");

            Assert.Equal((int)HttpStatusCode.OK, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnNoContentStatusCodeOnDeleteReview()
        {
            User user = await InsertUserOnDatabase();
            Review insertedReview = await InsertReviewOnDatabase();
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(user.Id));

            HttpResponseMessage response = await _httpClient.DeleteAsync($"../reviews/{insertedReview.Id}");

            Assert.Equal((int)HttpStatusCode.NoContent, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnNoContentStatusCodeOnCallDeleteInNotExistsReview()
        {
            User user = await InsertUserOnDatabase();
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(user.Id));

            HttpResponseMessage response = await _httpClient.DeleteAsync($"../reviews/{Guid.NewGuid()}");

            Assert.Equal((int)HttpStatusCode.NoContent, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnUnauthorizedOnCallDelete()
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"../reviews/{Guid.NewGuid()}");

            Assert.Equal((int)HttpStatusCode.Unauthorized, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnOkStatusCodeOnCallUpdate()
        {
            User user = await InsertUserOnDatabase();
            Review insertedReview = await InsertReviewOnDatabase();
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(user.Id));

            HttpResponseMessage response = await _httpClient.PutAsync($"../reviews/{insertedReview.Id}", CreateReviewMultipartFormContent());

            Assert.Equal((int)HttpStatusCode.OK, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnBadRequestCodeOnCallUpdateInNotExistsReview()
        {
            User user = await InsertUserOnDatabase();
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(user.Id));

            HttpResponseMessage response = await _httpClient.PutAsync($"../reviews/{Guid.NewGuid()}", CreateReviewMultipartFormContent());

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnForbiddenCodeOnCallUpdateReviewWithCreatorDifferentThatAuthenticatedUser()
        {
            User user = await InsertUserOnDatabase();
            Review insertedReview = await InsertReviewOnDatabase(user.Id);
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(Guid.NewGuid()));

            HttpResponseMessage response = await _httpClient.PutAsync($"../reviews/{insertedReview.Id}", CreateReviewMultipartFormContent());

            Assert.Equal((int)HttpStatusCode.Forbidden, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnUnauthorizedOnCallUpdate()
        {
            HttpResponseMessage response = await _httpClient.PutAsync($"../reviews/{Guid.NewGuid()}", CreateReviewMultipartFormContent());

            Assert.Equal((int)HttpStatusCode.Unauthorized, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnOkStatusCodeOnCallGetById()
        {
            User user = await InsertUserOnDatabase();
            Review insertedReview = await InsertReviewOnDatabase(user.Id);
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(Guid.NewGuid()));

            HttpResponseMessage response = await _httpClient.GetAsync($"../reviews/{insertedReview.Id}");

            Assert.Equal((int)HttpStatusCode.OK, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnBadRequestOnCallGetByIdOnNotFoundReview()
        {
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(Guid.NewGuid()));

            HttpResponseMessage response = await _httpClient.GetAsync($"../reviews/{Guid.NewGuid()}");

            Assert.Equal((int)HttpStatusCode.BadRequest, (int)response.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnUnauthorizedtOnCallGetById()
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"../reviews/{Guid.NewGuid()}");

            Assert.Equal((int)HttpStatusCode.Unauthorized, (int)response.StatusCode);
        }
    }
}
