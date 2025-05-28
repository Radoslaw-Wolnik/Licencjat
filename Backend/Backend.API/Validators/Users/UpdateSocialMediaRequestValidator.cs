using Backend.API.DTOs.Users;
using FluentValidation;

namespace Backend.API.Validators.Users;

public class UpdateSocialMediaRequestValidator : AbstractValidator<UpdateSocialMediaRequest>
{
    public UpdateSocialMediaRequestValidator()
    {
        RuleFor(x => x.Platform).IsInEnum().When(x => x.Platform.HasValue);
        RuleFor(x => x.Url).MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Url));
    }
}