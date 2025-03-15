// Domain/Entities/User.cs
namespace Backend.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public DateTime BirthDate { get; private set; }
    public Address? Address { get; private set; }

    public User(
        string email,
        string passwordHash,
        string firstName,
        string lastName,
        DateTime birthDate)
    {
        Validate(email, firstName, lastName, birthDate);
        
        Email = email;
        PasswordHash = passwordHash;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
    }

    private void Validate(
        string email,
        string firstName,
        string lastName,
        DateTime birthDate)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));
        
        if (!new System.ComponentModel.DataAnnotations.EmailAddressAttribute().IsValid(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        if (string.IsNullOrWhiteSpace(firstName) || firstName.Length > 50)
            throw new ArgumentException("First name must be between 1-50 characters", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName) || lastName.Length > 50)
            throw new ArgumentException("Last name must be between 1-50 characters", nameof(lastName));

        if (birthDate > DateTime.Now.AddYears(-13))
            throw new ArgumentException("User must be at least 13 years old", nameof(birthDate));
    }
}