using Backend.Application.Commands.GeneralBooks.Core;
using FluentValidation;

namespace Backend.Application.Validators.Commands.GeneralBook;

public sealed class CreateValidator : AbstractValidator<CreateGeneralBookCommand>
{
    public CreateValidator()
    {
        RuleFor(x => x.Title).NotEmpty();

        RuleFor(x => x.Author).NotEmpty();

        RuleFor(x => x.Published)
            .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));

        RuleFor(x => x.OryginalLanguage).NotEmpty();

        RuleFor(x => x.CoverFileName)
            .NotEmpty()
            .Matches(@".+\.(jpg|jpeg|png)$")
            .WithMessage("File must be a .jpg, .jpeg or .png");
    }
}