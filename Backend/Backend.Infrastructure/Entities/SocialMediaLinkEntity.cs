// Backend.Infrastructure/Entities/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Entities;

public class SocialMediaLinkEntity
{
    public Guid Id { get; set; }
    
    // info
    public string Platform { get; set; } = null!;
    public string Url { get; set; } = null!;


    // references
    public Guid UserId {get; set; }
    [Required]
    public virtual UserEntity User { get; set; } = null!;
}