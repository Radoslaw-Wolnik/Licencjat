using System.ComponentModel.DataAnnotations;
using Backend.Domain.Enums;

namespace Backend.Infrastructure.Entities;

public class FeedbackEntity
{
    public Guid Id { get; set; }
    
    // info
    [Required]
    [Range(1, 5)]
    public int Stars { get; set; } // 1-5
    [Required]
    
    public bool Recommend { get; set; } // would you or would you not recomend

    // statuses
    public SwapLength Lenght { get; set; }
    public SwapConditionBook ConditionBook { get; set; }
    public SwapCommunication Communication { get; set; }

    // references
    public Guid UserId { get; set; }
    public Guid SubSwapId { get; set; }
    [Required]
    public virtual UserEntity User { get; set; } = null!;
    [Required]
    public virtual SubSwapEntity SubSwap { get; set; } = null!;
}
