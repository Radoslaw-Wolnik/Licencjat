// Backend.Infrastructure/Entities/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Infrastructure.Data.Attributes;

namespace Backend.Infrastructure.Entities;

[HasUpdatedAt]
public class SubSwap
{
    public Guid Id { get; set; }
    
    // info
    public int PageAt { get; set; } = 0; // page that user a is currently at
    public bool IsRequesting { get; set; } // is this the requesting or the accepting user

    // Many To One
    public Guid SwapId { get; set; }
    public Guid UserId { get; set; } // person that requests the swap
    public Guid UserBookReadingId { get; set; } // book that person A reads - owned by the other user
    public Guid? FeedbackId { get; set; }
    public Guid? IssueId { get; set; }

    [Required]
    public virtual Swap Swap { get; set; } = null!;
    [Required]
    public virtual ApplicationUser User { get; set; } = null!;
    [Required]
    public virtual UserBook UserBookReading { get; set; } = null!;
    [ForeignKey("FeedbackId")]

    public virtual Feedback? Feedback { get; set; } // do we need ids to them as well? they are not required becouse at beggingin there wont be any
    [ForeignKey("IssueId")]
    public virtual Issue? Issue { get; set; } // hopefuly wont be here
}