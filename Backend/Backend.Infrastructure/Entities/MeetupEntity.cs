using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Domain.Enums;
using Backend.Infrastructure.Data.Attributes;

namespace Backend.Infrastructure.Entities;

[HasUpdatedAt]
public class MeetupEntity
{
    public Guid Id { get; set; }
    
    // info
    [Column(TypeName = "float(24)")] // SQL float(24) ≈ C# double
    public double Location_X { get; set; }
    [Column(TypeName = "float(24)")]
    public float Location_Y { get; set; }

    // status 
    public MeetupStatus Status { get; set; }
    

    // references
    public Guid SuggestedUserId { get; set; }
    public Guid SwapId { get; set; }
    [Required]
    public virtual UserEntity User { get; set; } = null!;
    [Required]
    public virtual SwapEntity Swap { get; set; } = null!;
}
