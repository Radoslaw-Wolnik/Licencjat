// Domain/User/User.cs
using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentResults;

namespace Backend.Domain.Entities;

public sealed class User : Entity<Guid>
{
    public string Email { get; } 
    public string Username { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public DateTime BirthDate { get; }

    private User(
        string email,
        string username,
        string firstName,
        string lastName,
        DateTime birthDate)
    {
        Email = email;
        Username = username;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
    }

    public static Result<User> Create(
        string email,
        string username,
        string firstName,
        string lastName,
        DateTime birthDate)
    {
        // Domain validation (business rules)
        var errors = new List<IError>();
        
        if (birthDate > DateTime.UtcNow.AddYears(-13))
            errors.Add(UserErrors.Underage);


        if (errors.Count != 0)
            return Result.Fail<User>(errors);

        var user = new User(email, username, firstName, lastName, birthDate);
        return Result.Ok(user);
    }
}