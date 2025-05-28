using Backend.API.DTOs.Swaps;
using FluentValidation;

namespace Backend.API.Validators.Swaps;

public class UpdateMeetupRequestValidator : AbstractValidator<UpdateMeetupRequest>
{
    public UpdateMeetupRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Latitude).InclusiveBetween(-90, 90).When(x => x.Latitude.HasValue);
        RuleFor(x => x.Longitude).InclusiveBetween(-180, 180).When(x => x.Longitude.HasValue);
    }
}