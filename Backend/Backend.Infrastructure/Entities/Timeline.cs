// Backend.Infrastructure/Entities/ApplicationUser.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

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

public enum TimelineStatus
{
    Requested,    
    Accepted,
    Declined,
    Canceled,
    [Display(Name = "Meeting up")]
    MeetingUp,
    [Display(Name = "Reding Books")]
    ReadingBooks,
    [Display(Name = "Finished reading books")]
    FinishedBooks,
    [Display(Name = "One person reading")]
    WaitingForFinish,
    Finished,
    [Display(Name = "Requested Finish ASAP")]
    RequestedFinish,
    Disputed, // if one person requests leaving bad feedback becouse some issues
}