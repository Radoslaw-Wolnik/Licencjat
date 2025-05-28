using Backend.API.DTOs.UserBooks;
using FluentValidation;

namespace Backend.API.Validators.UserBooks;

public class UpdateUserBookRequestValidator : AbstractValidator<UpdateUserBookRequest>
{
    public UpdateUserBookRequestValidator()
    {
        RuleFor(x => x.Status).IsInEnum().When(x => x.Status.HasValue);
        RuleFor(x => x.State).IsInEnum().When(x => x.State.HasValue);
    }
}