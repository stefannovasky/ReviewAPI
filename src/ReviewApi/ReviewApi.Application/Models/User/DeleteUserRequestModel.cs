namespace ReviewApi.Application.Models.User
{
    public class DeleteUserRequestModel
    {
        public string Password { get; set; }
        public string PasswordConfirmation { get; set; }
    }
}
