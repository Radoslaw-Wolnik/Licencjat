namespace Backend.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    // Add other domain properties
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}