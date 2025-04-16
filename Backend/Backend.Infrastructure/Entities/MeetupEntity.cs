// Backend.Infrastructure/Entities/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Domain.Enums;
using Backend.Infrastructure.Data.Attributes;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Entities;

[HasUpdatedAt]
public class MeetupEntity
{
    public Guid Id { get; set; }
    
    // info
    [Column(TypeName = "float(24)")] // SQL float(24) ≈ C# double
    public double? Location_X { get; set; }
    [Column(TypeName = "float(24)")]
    public float? Location_Y { get; set; }

    // status 
    public MeetupStatus Status { get; set; }
    

    // references
    public Guid SuggestedUserId { get; set; } // could we just have id of user that is suggesting meeting and mby the user that should agree idk? But we dont need entire user reference here
    public Guid SwapId { get; set; }
    [Required]
    public virtual UserEntity User { get; set; } = null!;
    [Required]
    public virtual SwapEntity Swap { get; set; } = null!;

}