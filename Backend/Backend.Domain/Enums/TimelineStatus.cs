using System.ComponentModel.DataAnnotations;
namespace Backend.Domain.Enums;

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