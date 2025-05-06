// Application/Validators/Auth/LoginRequestValidator.cs
using FluentValidation;
using Backend.Application.DTOs.Commands.Auth;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.Validators.Commands.Auth;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.UsernameOrEmail)
            .NotEmpty().WithMessage("Username or Email is required")
            .Must(BeValidEmailOrUsername).WithMessage("Invalid format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required");
    }

    private bool BeValidEmailOrUsername(string input)
    {
        if (input.Contains("@"))
            return new EmailAddressAttribute().IsValid(input);
        
        // Add username format validation if needed
        return !string.IsNullOrWhiteSpace(input);
    }
}