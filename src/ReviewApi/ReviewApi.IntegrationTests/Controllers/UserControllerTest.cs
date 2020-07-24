using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using ReviewApi.Application.Models.User;
using ReviewApi.Infra.Context;
using ReviewApi.IntegrationTests.CustomWebApplicationFactory;
using ReviewApi.IntegrationTests.Extensions;
using ReviewApi.IntegrationTests.Helpers;
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

        public UserControllerTest(CustomWebApplicationFactory<Startup> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
            _httpClient = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions());
            _createRequestHelper = new CreateRequestHelper();

            SqliteConnection connection = new SqliteConnection("Filename=:memory:");
            connection.Open();

            DbContextOptions<MainContext> options = new DbContextOptionsBuilder<MainContext>()
                .UseSqlite(connection)
                .Options;

            _database = new MainContext(options);
        }

        [Fact]
        public async Task ShouldReturnCreatedAtRouteOnCallCreate()
        {
            CreateUserRequestModel model = new CreateUserRequestModel() { Email = "user@mail.com", Name = "User Name", Password = "User Password" };
            HttpResponseMessage httpResponse = await _httpClient.PostAsync("../api/users", _createRequestHelper.CreateStringContent(model));

            Assert.Equal((int)HttpStatusCode.Created, (int)httpResponse.StatusCode);
            _database.ResetDatabase();
        }
    }
}
