using System.ComponentModel.DataAnnotations;
namespace Backend.Domain.Enums;

public enum MeetupStatus
{
    [Display(Name = "No location selected")]
    NoLocation,
    [Display(Name = "Changed the Location")]
    ChangedLocation,
    Proposed,
    Confirmed,
    Completed
}