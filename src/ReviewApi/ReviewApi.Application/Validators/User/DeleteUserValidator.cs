using FluentValidation;
using ReviewApi.Application.Models.User;

namespace ReviewApi.Application.Validators.User
{
    public class DeleteUserValidator : AbstractValidator<DeleteUserRequestModel>
    {
        public DeleteUserValidator()
        {
            RuleFor(u => u.Password)
                .NotNull().WithMessage("password cannot be null.")
                .NotEmpty().WithMessage("password cannot be empty.")
                .MinimumLength(8).WithMessage("password must be greater than 8 characters.")
                .MaximumLength(60).WithMessage("password must be less than 60 characters.");
            RuleFor(u => u.PasswordConfirmation)
                .Equal(u => u.Password).WithMessage("password confirmation must be equal to new password.");
        }
    }
}
