using System.ComponentModel.DataAnnotations;
namespace Backend.Domain.Enums;

public enum BookStatus
{
    Finished,
    Reading,
    Waiting,
    [Display(Name = "Coffee Table Decoration")]
    CoffeeTableDecoration
}