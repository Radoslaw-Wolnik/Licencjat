using System.ComponentModel.DataAnnotations;
namespace Backend.Domain.Enums;

public enum BookState
{
    Borrowed,
    Available,
    [Display(Name = "Not Owned")]
    NotOwned,
    [Display(Name = "Not Available")]
    NotAvailable
}