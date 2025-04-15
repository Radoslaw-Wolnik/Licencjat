// Backend.Infrastructure/Entities/Feedback.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Entities;

public class Feedback
{
    public Guid Id { get; set; }
    
    // info
    [Required]
    [Range(1, 5)]
    public int Stars { get; set; } // 1-5
    [Required]
    
    public bool Recommend { get; set; } // would you or would you not recomend

    // statuses
    [Column(TypeName = "nvarchar(24)")]
    public SwapLenght Lenght { get; set; }
    
    [Column(TypeName = "nvarchar(24)")]
    public SwapConditionBook ConditionBook { get; set; }
    [Column(TypeName = "nvarchar(24)")]
    public SwapCommunication Communication { get; set; }

    // references
    public Guid UserId { get; set; }
    public Guid SubSwapId { get; set; }
    [Required]
    public virtual ApplicationUser User { get; set; } = null!;
    public virtual SubSwap? SubSwap { get; set; } // nullable foreign key structure, ofc it will always be here not null

}

public enum SwapLenght
{
    [Display(Name = "Just right")]
    JustRigh,
    [Display(Name = "Too long")]
    TooLong,
    [Display(Name = "Too short")]
    TooShort,
}

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

public enum SwapCommunication
{
    Okay,
    Slow,
    Perfect,
    [Display(Name = "Too much")]
    TooMuch,
    Problematic
}
