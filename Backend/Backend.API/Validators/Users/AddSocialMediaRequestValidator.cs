using Backend.API.DTOs.Users;
using FluentValidation;

namespace Backend.API.Validators.Users;

public class AddSocialMediaRequestValidator : AbstractValidator<AddSocialMediaRequest>
{
    public AddSocialMediaRequestValidator()
    {
        RuleFor(x => x.Platform).IsInEnum();
        RuleFor(x => x.Url).NotEmpty().MaximumLength(200);
    }
}
