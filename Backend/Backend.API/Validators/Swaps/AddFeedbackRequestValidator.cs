using Backend.API.DTOs.Swaps;
using FluentValidation;

namespace Backend.API.Validators.Swaps;

public class AddFeedbackRequestValidator : AbstractValidator<AddFeedbackRequest>
{
    public AddFeedbackRequestValidator()
    {
        RuleFor(x => x.Stars).InclusiveBetween(1, 5);
        RuleFor(x => x.Length).IsInEnum();
        RuleFor(x => x.Condition).IsInEnum();
        RuleFor(x => x.Communication).IsInEnum();
    }
}
