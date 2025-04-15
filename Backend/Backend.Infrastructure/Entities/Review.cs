using System.ComponentModel.DataAnnotations;
using Backend.Infrastructure.Data.Attributes;

namespace Backend.Infrastructure.Entities;

[HasUpdatedAt]
public class Review 
{
    public Guid Id { get; set; }
    
    [Range(1, 10)]

    public int Rating { get; set; } // 1-10
    public string? Comment { get; set; }

    // references
    public Guid UserId { get; set; }
    public Guid BookId { get; set; }

    public virtual ApplicationUser User { get; set; } = null!;
    public virtual GeneralBook Book { get; set; } = null!;
}