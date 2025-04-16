// Backend.Infrastructure/Entities/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Domain.Errors;
using Backend.Infrastructure.Data.Attributes;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Entities;

[HasUpdatedAt]
public class UserEntity : IdentityUser<Guid>
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
    [Range(1.0, 5.0)]
    public float Reputation { get; set; } = 4.0f; // 1-5

    // RelationShips
    public virtual ICollection<UserBookEntity> UserBooks { get; set; } = [];
    public virtual ICollection<ReviewEntity> Reviews { get; set; } = [];
    public virtual ICollection<SocialMediaLinkEntity> SocialMediaLinks { get; set; } = [];

    
    // Many to Many relations
    public virtual ICollection<GeneralBookEntity> Wishlist { get; set; } = [];
    public virtual ICollection<GeneralBookEntity> FollowedBooks { get; set; } = [];

    public virtual ICollection<UserFollowingEntity> Following { get; set; } = [];
    public virtual ICollection<UserFollowingEntity> Followers { get; set; } = [];
    public virtual ICollection<UserBlockedEntity> BlockedUsers { get; set; } = [];

    public virtual ICollection<SubSwapEntity> SubSwaps { get; set; } = [];
    public virtual ICollection<MeetupEntity> Meetups { get; set; } = [];
    public virtual ICollection<FeedbackEntity> SwapsFeedbacks { get; set; } = [];
    public virtual ICollection<TimelineEntity> SwapsTimelineupdates { get; set; } = [];
    public virtual ICollection<IssueEntity> SwapsIssues { get; set; } = [];
}