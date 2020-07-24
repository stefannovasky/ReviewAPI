namespace ReviewApi.Application.Models.User
{
    public class AuthenticationUserResponseModel
    {
        public UserResponseModel User { get; set; }
        public string Token { get; set; }
    }
}
