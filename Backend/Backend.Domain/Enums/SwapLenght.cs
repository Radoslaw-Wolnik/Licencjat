using System.ComponentModel.DataAnnotations;
namespace Backend.Domain.Enums;

public enum SwapLenght
{
    [Display(Name = "Just right")]
    JustRigh,
    [Display(Name = "Too long")]
    TooLong,
    [Display(Name = "Too short")]
    TooShort,
}