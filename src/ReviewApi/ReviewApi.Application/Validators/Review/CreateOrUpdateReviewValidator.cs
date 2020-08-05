using FluentValidation;
using ReviewApi.Application.Models.Review;

namespace ReviewApi.Application.Validators.Review
{
    public class CreateOrUpdateReviewValidator : AbstractValidator<CreateOrUpdateReviewRequestModel>
    {
        public CreateOrUpdateReviewValidator()
        {
            RuleFor(p => p.Title)
                .NotNull().WithMessage("review title cannot be null.")
                .NotEmpty().WithMessage("review title cannot be empty.")
                .MinimumLength(2).WithMessage("review title must be greater than 1 characters.")
                .MaximumLength(150).WithMessage("review title must be less than 151 characters.");
            RuleFor(p => p.Text)
                .NotNull().WithMessage("review text cannot be null.")
                .NotEmpty().WithMessage("review text cannot be empty.")
                .MinimumLength(20).WithMessage("review text must be greater than 20 characters.")
                .MaximumLength(1500).WithMessage("review text must be less than 1501 characters.");
            RuleFor(p => p.Stars)
                .NotNull().WithMessage("review stars cannot be null.")
                .GreaterThan(0).WithMessage("review stars must be greater than zero.")
                .LessThan(6).WithMessage("review stars must be less than 6.");
            RuleFor(p => p.Image)
                .NotNull().WithMessage("review stars image cannot be null.");
        }
    }
}
