using System.ComponentModel.DataAnnotations;
namespace Backend.Domain.Enums;

public enum SwapConditionBook
{
    Same,
    Worse,
    [Display(Name = "Very very bad")]
    VeryVeryBad,
    [Display(Name = "Somehow better")]
    Better,
    Destroyed
}