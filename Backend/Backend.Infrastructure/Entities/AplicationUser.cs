// Backend.Infrastructure/Entities/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Domain.Errors;
using Backend.Infrastructure.Data.Attributes;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Entities;

[HasUpdatedAt]
public class ApplicationUser : IdentityUser<Guid>
{
    // username and email is inherited from IdentityUser
    [Required, MaxLength(50)]
    public string FirstName { get; set; } = null!;
    [Required, MaxLength(50)]
    public string LastName { get; set; } = null!;
    [Required]
    public DateOnly BirthDate { get; set; } // changed to DateOnly
    [Required]
    public string City { get; set; } = null!; 
    [Required]
    public string Country { get; set; } = null!;
    public string? ProfilePicture { get; set; } = null;
    public string? Bio { get; set; } = null;
    
    [Column(TypeName = "decimal(4,3)")] // 4 digits total (e.g., 5.000)
    public float Reputation { get; set; } = 4; // 1-5

    // RelationShips
    public virtual ICollection<UserBook> UserBooks { get; set; } = [];
    public virtual ICollection<Review> Reviews { get; set; } = [];
    public virtual ICollection<SocialMediaLink> SocialMediaLinks { get; set; } = [];

    
    // Many to Many relations
    public virtual ICollection<GeneralBook> Wishlist { get; set; } = [];
    public virtual ICollection<GeneralBook> FollowedBooks { get; set; } = [];

    public virtual ICollection<UserFollowing> Following { get; set; } = [];
    public virtual ICollection<UserFollowing> Followers { get; set; } = [];
    public virtual ICollection<UserBlocked> BlockedUsers { get; set; } = [];

    public virtual ICollection<SubSwap> SubSwaps { get; set; } = [];
    public virtual ICollection<Meetup> Meetups { get; set; } = [];
    public virtual ICollection<Feedback> SwapsFeedbacks { get; set; } = [];
    public virtual ICollection<Timeline> SwapsTimelineupdates { get; set; } = [];
    public virtual ICollection<Issue> SwapsIssues { get; set; } = [];
}