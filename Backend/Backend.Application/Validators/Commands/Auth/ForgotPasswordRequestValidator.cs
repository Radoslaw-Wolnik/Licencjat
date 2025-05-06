// Application/Validators/Auth/ForgotPasswordRequestValidator.cs
using FluentValidation;
using Backend.Application.DTOs.Commands.Auth;

namespace Backend.Application.Validators.Commands.Auth;

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordRequest>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}