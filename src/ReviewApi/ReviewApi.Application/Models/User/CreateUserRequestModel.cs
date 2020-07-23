namespace ReviewApi.Application.Models.User
{
    public class CreateUserRequestModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }
}
