using Backend.API.DTOs.Users;
using FluentValidation;

namespace Backend.API.Validators.Users;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.CountryCode).Length(2);
        RuleFor(x => x.Bio).MaximumLength(500);
    }
}
