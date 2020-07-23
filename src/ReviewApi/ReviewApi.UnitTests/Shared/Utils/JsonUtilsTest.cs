using System;
using ReviewApi.Domain.Entities;
using ReviewApi.Shared.Interfaces;
using ReviewApi.Shared.Utils;
using Xunit;

namespace ReviewApi.UnitTests.Shared.Utils
{
    public class JsonUtilsTest
    {
        private readonly IJsonUtils _jsonUtils; 
        public JsonUtilsTest()
        {
            _jsonUtils = new JsonUtils();
        }

        [Fact]
        public void ShouldSerializeObject()
        {
            object obj = new { Key = "Value" };

            string json = _jsonUtils.Serialize(obj);

            Assert.Equal("{\"Key\":\"Value\"}", json);
        }

        [Fact]
        public void ShouldDeserializeObject()
        {
            Guid guid = Guid.NewGuid();
            BaseEntity obj = new BaseEntity(guid);

            string json = _jsonUtils.Serialize(obj);
            BaseEntity deserialized = _jsonUtils.Deserialize<BaseEntity>(json);

            Assert.Equal(guid, deserialized.Id);
        }
    }
}
