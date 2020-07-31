namespace ReviewApi.Application.Models.User
{
    public class ResetPasswordUserRequestModel
    {
        public string Code { get; set; }
        public string NewPassword { get; set; }
        public string NewPasswordConfirmation { get; set; }
    }
}
