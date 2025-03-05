// Backend.Infrastructure/Entities/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    // Add custom properties here if needed
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}