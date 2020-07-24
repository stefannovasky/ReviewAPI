using FluentValidation;
using ReviewApi.Application.Models.User;

namespace ReviewApi.Application.Validators
{
    public class ConfirmUserValidator : AbstractValidator<ConfirmUserRequestModel>
    {
        public ConfirmUserValidator()
        {
            RuleFor(p => p.Code)
                .NotNull().WithMessage("code cannot be null.")
                .NotEmpty().WithMessage("code cannot be empty.")
                .Length(8).WithMessage("code must be 8 characters.");
        }
    }
}
