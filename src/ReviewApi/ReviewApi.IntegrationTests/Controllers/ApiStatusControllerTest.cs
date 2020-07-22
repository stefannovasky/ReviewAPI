using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using ReviewApi.IntegrationTests.CustomWebApplicationFactory;
using Xunit;

namespace ReviewApi.IntegrationTests.Controllers
{
    [Collection("Sequential")]
    public class ApiStatusControllerTest : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly CustomWebApplicationFactory<Startup> _webApplicationFactory;
        private readonly HttpClient _httpClient;

        public ApiStatusControllerTest(CustomWebApplicationFactory<Startup> webApplicationFactory)
        {
            _webApplicationFactory = webApplicationFactory;
            _httpClient = _webApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions());
        }

        [Fact]
        public async Task ShouldReturnOkStatusCodeOnCallApiStatus()
        {
            HttpResponseMessage httpResponse = await _httpClient.GetAsync("../api/");

            Assert.Equal((int)HttpStatusCode.OK, (int)httpResponse.StatusCode);
        }
    }
}
