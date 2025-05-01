using System.ComponentModel.DataAnnotations;
using Backend.Domain.Enums;

namespace Backend.Infrastructure.Entities;

public class TimelineEntity
{
    public Guid Id { get; set; }
    
    // info
    [Required]
    public string Description { get; set; } = null!;

    // status 
    public TimelineStatus Status { get; set; }
    

    // references
    public Guid UserId { get; set; }
    public Guid SwapId { get; set; }
    [Required]
    public virtual UserEntity User { get; set; } = null!;
    [Required]
    public virtual SwapEntity Swap { get; set; } = null!;
}