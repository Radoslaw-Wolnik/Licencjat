using Backend.API.DTOs.GeneralBooks;
using Backend.Domain.Common;
using FluentValidation;

namespace Backend.API.Validators.GeneralBooks;

public sealed class CreateGeneralBookRequestValidator 
    : AbstractValidator<CreateGeneralBookRequest>
{
    public CreateGeneralBookRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Published).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today));
        RuleFor(x => x.OriginalLanguage).NotEmpty().IsEnumName(typeof(LanguageCode));
        RuleFor(x => x.CoverFileName).NotEmpty().MaximumLength(256);

        /* logic rule - should be in the command not here
        RuleFor(x => x.CoverFileName)
            .NotEmpty()
            .Matches(@".+\.(jpg|jpeg|png)$")
            .WithMessage("File must be a .jpg, .jpeg or .png"); */
    }
}