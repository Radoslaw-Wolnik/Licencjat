// Backend.Infrastructure/Entities/GeneralBook.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Domain.Entities;
using Backend.Domain.Errors;

namespace Backend.Infrastructure.Entities;

public class GeneralBook
{
    public Guid Id { get; set; }
    // info
    [Required]
    public string Title { get; set; } = null!;
    [Required]
    public string Author { get; set; } = null!;
    [Required]
    public DateOnly Published { get; set; }
    [Required, MaxLength(5)]
    public string Language { get; set; } = null!; // oryginal language when published
    [Required]
    public string CoverPhoto { get; set; } = null!;

    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    [Column(TypeName = "decimal(4,3)")]
    public float? ReviewAverage { get; set; } // calculated based on reviews but easier to fetch then calculate every time

    // references
    public virtual ICollection<UserBook> UserBooks { get; set; } = [];
    public virtual ICollection<Review> Reviews { get; set; } = [];
    public virtual ICollection<ApplicationUser> WishlistedByUsers { get; set; } = [];
    public virtual ICollection<ApplicationUser> FollowedByUsers { get; set; } = [];
}