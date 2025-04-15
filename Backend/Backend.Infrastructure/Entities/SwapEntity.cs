// Backend.Infrastructure/Entities/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Infrastructure.Data.Attributes;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Entities;

[HasUpdatedAt]
public class SwapEntity
{
    public Guid Id { get; set; }

    // Many To One
    public Guid SubSwapRequestingId { get; set; } // person that requests the swap
    public Guid? SubSwapAcceptingId { get; set; } // person that agrees for swap
    [Required]
    public virtual SubSwapEntity SubSwapRequesting { get; set; } = null!;
    [ForeignKey("SubSwapAcceptingId")]
    public virtual SubSwapEntity? SubSwapAccepting { get; set; }

    // two/many to One
    public virtual ICollection<MeetupEntity> Meetups { get; set; } = []; // 2
    public virtual ICollection<TimelineEntity> TimelineUpdates { get; set; } = []; // every timeline update
}