using Backend.API.DTOs.UserBooks;
using FluentValidation;

namespace Backend.API.Validators.UserBooks;

public class UpdateBookmarkRequestValidator : AbstractValidator<UpdateBookmarkRequest>
{
    public UpdateBookmarkRequestValidator()
    {
        RuleFor(x => x.Colour).IsInEnum().When(x => x.Colour.HasValue);
        RuleFor(x => x.Page).GreaterThan(0).When(x => x.Page.HasValue);
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description != null);
    }
}