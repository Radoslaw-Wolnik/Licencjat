using Backend.API.DTOs.UserBooks;
using FluentValidation;

namespace Backend.API.Validators.UserBooks;

public class CreateBookmarkRequestValidator : AbstractValidator<CreateBookmarkRequest>
{
    public CreateBookmarkRequestValidator()
    {
        RuleFor(x => x.Colour).IsInEnum();
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description != null);
    }
}