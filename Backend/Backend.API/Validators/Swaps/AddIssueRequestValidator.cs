using Backend.API.DTOs.Swaps;
using FluentValidation;

namespace Backend.API.Validators.Swaps;

public class AddIssueRequestValidator : AbstractValidator<AddIssueRequest>
{
    public AddIssueRequestValidator()
    {
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
    }
}
