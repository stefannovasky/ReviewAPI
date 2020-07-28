using FluentValidation;
using ReviewApi.Application.Models.User;

namespace ReviewApi.Application.Validators.User
{
    public class UpdatePasswordUserValidator : AbstractValidator<UpdatePasswordUserRequestModel>
    {
        public UpdatePasswordUserValidator()
        {
            RuleFor(u => u.OldPassword)
                .NotNull().WithMessage("old password cannot be null.")
                .NotEmpty().WithMessage("old password cannot be empty.")
                .MinimumLength(8).WithMessage("old password must be greater than 8 characters.")
                .MaximumLength(60).WithMessage("old password must be less than 60 characters.");
            RuleFor(u => u.NewPassword)
                .NotNull().WithMessage("new password cannot be null.")
                .NotEmpty().WithMessage("new password cannot be empty.")
                .MinimumLength(8).WithMessage("new password must be greater than 8 characters.")
                .MaximumLength(60).WithMessage("new password must be less than 60 characters.");
            RuleFor(u => u.NewPasswordConfirmation)
                .Equal(u => u.NewPassword).WithMessage("new password confirmation must be equal to new password.");
        }
    }
}
