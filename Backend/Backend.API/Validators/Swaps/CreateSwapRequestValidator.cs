using Backend.API.DTOs.Swaps;
using FluentValidation;

namespace Backend.API.Validators.Swaps;

public class CreateSwapRequestValidator : AbstractValidator<CreateSwapRequest>
{
    public CreateSwapRequestValidator()
    {
        RuleFor(x => x.UserAcceptingId).NotEmpty();
        RuleFor(x => x.RequestedBookId).NotEmpty();
    }
}
