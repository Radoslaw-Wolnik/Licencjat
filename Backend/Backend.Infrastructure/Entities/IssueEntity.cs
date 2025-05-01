using System.ComponentModel.DataAnnotations;

namespace Backend.Infrastructure.Entities;

public class IssueEntity // or report or sth like that
{
    public Guid Id { get; set; }
    
    // info
    [Required]
    public string Description { get; set; } = null!; // describe your experience with another user
    

    // references
    public Guid UserId { get; set; }
    public Guid SubSwapId { get; set; }
    [Required]
    public virtual UserEntity User { get; set; } = null!;
    [Required]
    public virtual SubSwapEntity SubSwap { get; set; } = null!;

}