// Backend.Infrastructure/Entities/Address.cs
using System.ComponentModel.DataAnnotations;

namespace Backend.Infrastructure.Entities;

public class Address
{
    public Guid Id { get; set; }
    
    [Required]
    public string Street { get; set; } = null!;
    
    [Required]
    public string City { get; set; } = null!;
    
    public Guid UserId { get; set; }
    // One-to-One Relationship with User
    public virtual ApplicationUser User { get; set; } = null!;
}