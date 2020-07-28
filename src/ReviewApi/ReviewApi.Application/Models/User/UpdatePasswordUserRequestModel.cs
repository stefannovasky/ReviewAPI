namespace ReviewApi.Application.Models.User
{
    public class UpdatePasswordUserRequestModel
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirmation { get; set; }
    }
}
