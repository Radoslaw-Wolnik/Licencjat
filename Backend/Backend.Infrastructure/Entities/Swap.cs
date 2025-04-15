// Backend.Infrastructure/Entities/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Infrastructure.Data.Attributes;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Entities;

[HasUpdatedAt]
public class Swap
{
    public Guid Id { get; set; }

    // Many To One
    public Guid SubSwapAId { get; set; } // person that requests the swap
    public Guid SubSwapBId { get; set; } // person that agrees for swap
    [Required]
    public virtual SubSwap SubSwapA { get; set; } = null!;
    [Required]
    public virtual SubSwap SubSwapB { get; set; } = null!;

    // two/many to One
    public virtual ICollection<Meetup> Meetups { get; set; } = []; // 2
    public virtual ICollection<Timeline> TimelineUpdates { get; set; } = []; // every timeline update
}