using System.ComponentModel.DataAnnotations;
using Backend.Infrastructure.Data.Attributes;

namespace Backend.Infrastructure.Entities;

[HasUpdatedAt]
public class SwapEntity
{
    public Guid Id { get; set; }

    // Many To One
    public Guid SubSwapRequestingId { get; set; } // person that requests the swap
    public Guid SubSwapAcceptingId { get; set; } // person that agrees for swap
    [Required]
    public virtual SubSwapEntity SubSwapRequesting { get; set; } = null!;
    [Required]
    public virtual SubSwapEntity SubSwapAccepting { get; set; } = null!;

    // back-ref for the FK in SubSwapEntity
    public virtual ICollection<SubSwapEntity> SubSwaps { get; set; } = new List<SubSwapEntity>();

    // two/many to One
    public virtual ICollection<MeetupEntity> Meetups { get; set; } = []; // 2
    public virtual ICollection<TimelineEntity> TimelineUpdates { get; set; } = []; // every timeline update
}
