// Backend.Application/DTOs/Auth/RegisterRequest.cs
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Auth;

public class RegisterRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email format")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8)]
    public string Password { get; set; } = null!;


    [Required(ErrorMessage = "FirstName is required")]
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "LastName is required")]
    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "BirthDate is required")]
    public DateTime BirthDate { get; set; }
}