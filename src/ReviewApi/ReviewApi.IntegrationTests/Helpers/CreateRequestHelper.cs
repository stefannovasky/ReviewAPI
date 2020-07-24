using System.Net.Http;
using System.Text;

namespace ReviewApi.IntegrationTests.Helpers
{
    internal class CreateRequestHelper
    {
        public StringContent CreateStringContent(object obj)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(obj);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}
