namespace ReviewApi.Shared.Interfaces
{
    public interface IJsonUtils
    {
        string Serialize(object obj);
        T Deserialize<T>(string json);
    }
}
