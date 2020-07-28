using FluentValidation;
using ReviewApi.Application.Models.User;

namespace ReviewApi.Application.Validators.User
{
    public class UpdateNameUserValidator : AbstractValidator<UpdateNameUserRequestModel>
    {
        public UpdateNameUserValidator()
        {
            RuleFor(u => u.Name)
               .NotNull().WithMessage("name cannot be null.")
               .NotEmpty().WithMessage("name cannot be empty.")
               .MinimumLength(3).WithMessage("name must be greater than 2 characters.")
               .MaximumLength(150).WithMessage("name must be less than 150 characters.");
        }
    }
}
