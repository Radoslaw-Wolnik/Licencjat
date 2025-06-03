using System.ComponentModel.DataAnnotations;
namespace Backend.Domain.Enums;

public enum SwapLength
{
    [Display(Name = "Just right")]
    JustRight,
    [Display(Name = "Too long")]
    TooLong,
    [Display(Name = "Too short")]
    TooShort,
}