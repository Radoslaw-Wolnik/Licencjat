// Backend.Infrastructure/Entities/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    [Required, MaxLength(50)]
    public string FirstName { get; set; } = null!;
    [Required, MaxLength(50)]
    public string LastName { get; set; } = null!;
    [Required]
    public DateTime BirthDate { get; set; }
    public float Reputation { get; set; } = 4; // Consider validation (1-5)

    // Address one user one address
    public virtual Address? Address { get; set; }

}