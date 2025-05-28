using Backend.API.DTOs.GeneralBooks;
using FluentValidation;

namespace Backend.API.Validators.GeneralBooks;

public sealed class ReviewRequestValidator 
    : AbstractValidator<ReviewRequest>
{
    public ReviewRequestValidator()
    {
        RuleFor(x => x.Rating).InclusiveBetween(1, 5);
        RuleFor(x => x.Comment).MaximumLength(1000).When(x => !string.IsNullOrEmpty(x.Comment));
    }
}