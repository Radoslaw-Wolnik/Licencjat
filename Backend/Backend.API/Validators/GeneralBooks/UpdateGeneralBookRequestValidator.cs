using Backend.API.DTOs.GeneralBooks;
using Backend.Domain.Common;
using FluentValidation;

namespace Backend.API.Validators.GeneralBooks;

public sealed class UpdateGeneralBookRequestValidator 
    : AbstractValidator<UpdateGeneralBookRequest>
{
    public UpdateGeneralBookRequestValidator()
    {
        RuleFor(x => x.Title).MaximumLength(100).When(x => x.Title != null);
        RuleFor(x => x.Author).MaximumLength(100).When(x => x.Author != null);
        RuleFor(x => x.Published).LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
                              .When(x => x.Published.HasValue);
        RuleFor(x => x.OriginalLanguage).IsEnumName(typeof(LanguageCode))
                              .When(x => !string.IsNullOrEmpty(x.OriginalLanguage));
    }
}