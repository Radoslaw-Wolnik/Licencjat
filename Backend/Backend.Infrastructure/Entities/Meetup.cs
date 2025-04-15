// Backend.Infrastructure/Entities/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Infrastructure.Data.Attributes;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Entities;

[HasUpdatedAt]
public class Meetup
{
    public Guid Id { get; set; }
    
    // info
    [Column(TypeName = "float(24)")] // SQL float(24) ≈ C# double
    public double? Location_X { get; set; }
    [Column(TypeName = "float(24)")]
    public float? Location_Y { get; set; }

    // status 
    [Column(TypeName = "nvarchar(24)")]
    public MeetupStatus Status { get; set; }
    

    // references
    public Guid SuggestedUserId { get; set; } // could we just have id of user that is suggesting meeting and mby the user that should agree idk? But we dont need entire user reference here
    public Guid SwapId { get; set; }
    [Required]
    public virtual ApplicationUser User { get; set; } = null!;
    [Required]
    public virtual Swap Swap { get; set; } = null!;

}

public enum MeetupStatus
{
    [Display(Name = "No location selected")]
    NoLocation,
    [Display(Name = "Changed the Location")]
    ChangedLocation,
    Waiting,
    Agreed,
    Completed
}