using System.ComponentModel.DataAnnotations;
using Backend.Domain.Enums;

namespace Backend.Infrastructure.Entities;

public class SocialMediaLinkEntity
{
    public Guid Id { get; set; }
    
    // info
    public SocialMediaPlatform Platform { get; set; }
    public string Url { get; set; } = null!;


    // references
    public Guid UserId {get; set; }
    [Required]
    public virtual UserEntity User { get; set; } = null!;
}
