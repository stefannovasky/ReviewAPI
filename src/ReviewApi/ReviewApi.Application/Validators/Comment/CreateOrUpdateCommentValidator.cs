using FluentValidation;
using ReviewApi.Application.Models.Comment;

namespace ReviewApi.Application.Validators.Comment
{
    public class CreateOrUpdateCommentValidator : AbstractValidator<CreateCommentRequestModel>
    {
        public CreateOrUpdateCommentValidator()
        {
            RuleFor(c => c.Text)
                .NotNull().WithMessage("comment text cannot be null.")
                .NotEmpty().WithMessage("comment text cannot be empty.")
                .MaximumLength(1500).WithMessage("comment text must be less than 1501 characters.");
        }
    }
}
