using Backend.API.DTOs.UserBooks;
using FluentValidation;

namespace Backend.API.Validators.UserBooks;

public class CreateUserBookRequestValidator : AbstractValidator<CreateUserBookRequest>
{
    public CreateUserBookRequestValidator()
    {
        RuleFor(x => x.BookId).NotEmpty();
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.State).IsInEnum();
        RuleFor(x => x.Language).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PageCount).GreaterThan(0);
        RuleFor(x => x.CoverFileName).NotEmpty().MaximumLength(256);
        /* logic rule - should be in the command not here
        RuleFor(x => x.CoverFileName)
            .NotEmpty()
            .Matches(@".+\.(jpg|jpeg|png)$")
            .WithMessage("File must be a .jpg, .jpeg or .png"); */
    }
}