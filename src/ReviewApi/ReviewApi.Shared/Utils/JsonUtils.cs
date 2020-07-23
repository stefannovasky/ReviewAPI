using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ReviewApi.Shared.Interfaces;

namespace ReviewApi.Shared.Utils
{
    public class JsonUtils : IJsonUtils
    {
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings()
            {
                ContractResolver = new NonPublicPropertiesResolver()
            });
        }

        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }

    internal class NonPublicPropertiesResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);
            if (member is PropertyInfo pi)
            {
                prop.Readable = (pi.GetMethod != null);
                prop.Writable = (pi.SetMethod != null);
            }
            return prop;
        }
    }
}
