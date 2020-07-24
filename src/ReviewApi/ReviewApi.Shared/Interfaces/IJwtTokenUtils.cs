namespace ReviewApi.Shared.Interfaces
{
    public interface IJwtTokenUtils
    {
        string GenerateToken(string id);
    }
}
