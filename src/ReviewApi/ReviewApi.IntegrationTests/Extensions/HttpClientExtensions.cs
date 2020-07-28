using System.Net.Http;

namespace ReviewApi.IntegrationTests.Extensions
{
    public static class HttpClientExtensions
    {
        public static void InsertAuthorizationTokenOnRequestHeader(this HttpClient client, string token)
        {
            client.DefaultRequestHeaders.Add("Authorization", token);
        }
    }
}
