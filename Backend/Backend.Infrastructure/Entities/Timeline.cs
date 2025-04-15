// Backend.Infrastructure/Entities/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Domain.Enums;

namespace Backend.Infrastructure.Entities;

public class Timeline
{
    public Guid Id { get; set; }
    
    // info
    [Required]
    public string Description { get; set; } = null!;

    // status 
    [Column(TypeName = "nvarchar(24)")]
    public TimelineStatus Status { get; set; }
    

    // references
    public Guid UserId { get; set; } // could we just have id of user that is suggesting meeting and mby the user that should agree idk? But we dont need entire user reference here
    public Guid SwapId { get; set; }
    [Required]
    public virtual ApplicationUser User { get; set; } = null!;
    [Required]
    public virtual Swap Swap { get; set; } = null!;
}