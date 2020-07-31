using FluentValidation;
using ReviewApi.Application.Models.User;

namespace ReviewApi.Application.Validators.User
{
    public class ForgotPasswordUserValidator : AbstractValidator<ForgotPasswordUserRequestModel>
    {
        public ForgotPasswordUserValidator()
        {
            RuleFor(u => u.Email)
               .NotNull().WithMessage("email cannot be null.")
               .NotEmpty().WithMessage("email cannot be empty.")
               .MaximumLength(254).WithMessage("email must be less than 254 characters.")
               .Matches(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z").WithMessage("email must be a valid email.");
        }
    }
}
