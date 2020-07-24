﻿using FluentValidation;
using FluentValidation.Validators;
using ReviewApi.Application.Models.User;

namespace ReviewApi.Application.Validators
{
    public class CreateUserValidator : AbstractValidator<CreateUserRequestModel>
    {
        public CreateUserValidator()
        {
            RuleFor(u => u.Email)
                .NotNull().WithMessage("email cannot be null.")
                .NotEmpty().WithMessage("email cannot be empty.")
                .MaximumLength(254).WithMessage("email must be less than 254 characters.")
                .Matches(@"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z").WithMessage("email must be a valid email.");
            RuleFor(u => u.Name)
                .NotNull().WithMessage("name cannot be null.")
                .NotEmpty().WithMessage("name cannot be empty.")
                .MinimumLength(3).WithMessage("name must be greater than 2 characters.")
                .MaximumLength(150).WithMessage("name must be less than 150 characters.");
            RuleFor(u => u.Password)
                .NotNull().WithMessage("password cannot be null.")
                .NotEmpty().WithMessage("password cannot be empty.")
                .MinimumLength(8).WithMessage("password must be greater than 8 characters.")
                .MaximumLength(60).WithMessage("password must be less than 60 characters.");
        }
    }
}
