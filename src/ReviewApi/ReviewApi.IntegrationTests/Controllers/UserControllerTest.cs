using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ReviewApi.Application.Models.User;
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
    public class UserControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _webApplicationFactory;
        private readonly HttpClient _httpClient;
        private readonly MainContext _database;
        private readonly CreateRequestHelper _createRequestHelper;
        private readonly AuthorizationTokenHelper _authorizationTokenHelper;

        public UserControllerTest(CustomWebApplicationFactory<Startup> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
            _httpClient = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions());
            _createRequestHelper = new CreateRequestHelper();
            _authorizationTokenHelper = new AuthorizationTokenHelper();

            DbContextOptions<MainContext> options = new DbContextOptionsBuilder<MainContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _database = new MainContext(options);
        }
        
        private async Task<Guid> InsertUserOnDatabase()
        {
            _database.Database.EnsureCreated();
            User user = new User("User Name", "user@mail.com", new HashUtils().GenerateHash("User password"));
            await _database.Set<User>().AddAsync(user);
            user.Confirm();
            await _database.SaveChangesAsync();
            return user.Id;
        }
        
        [Fact]
        public async Task ShouldReturnCreatedAtRouteOnCallCreate()
        {
            CreateUserRequestModel model = new CreateUserRequestModel() { Email = "user@mail.com", Name = "User Name", Password = "User Password" };
            HttpResponseMessage httpResponse = await _httpClient.PostAsync("../users", _createRequestHelper.CreateStringContent(model));

            Assert.Equal((int)HttpStatusCode.Created, (int)httpResponse.StatusCode);
            _database.ResetDatabase();
        }

        [Fact]
        public async Task ShouldReturnOkOnCallUpdateUserName()
        {
            Guid id = await InsertUserOnDatabase();
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(id));
            UpdateNameUserRequestModel model = new UpdateNameUserRequestModel() { Name = "User Name" };

            HttpResponseMessage httpResponse = await _httpClient.PutAsync("../users/name", _createRequestHelper.CreateStringContent(model));

            Assert.Equal((int)HttpStatusCode.OK, (int)httpResponse.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnOkOnCallUpdateUserPassword()
        {
            Guid id = await InsertUserOnDatabase();
            _httpClient.InsertAuthorizationTokenOnRequestHeader(_authorizationTokenHelper.CreateToken(id));
            UpdatePasswordUserRequestModel model = new UpdatePasswordUserRequestModel() { NewPassword = "NEWPASSWORD", NewPasswordConfirmation = "NEWPASSWORD", OldPassword = "User password" };

            HttpResponseMessage httpResponse = await _httpClient.PutAsync("../users/password", _createRequestHelper.CreateStringContent(model));

            Assert.Equal((int)HttpStatusCode.OK, (int)httpResponse.StatusCode);
        }
    }
}
