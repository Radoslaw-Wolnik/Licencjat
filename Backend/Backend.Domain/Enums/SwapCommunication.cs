using System.ComponentModel.DataAnnotations;
namespace Backend.Domain.Enums;

public enum SwapCommunication
{
    Okay,
    Slow,
    Perfect,
    [Display(Name = "Too much")]
    TooMuch,
    Problematic
}