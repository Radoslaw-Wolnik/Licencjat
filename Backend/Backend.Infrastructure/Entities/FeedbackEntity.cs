// Backend.Infrastructure/Entities/Feedback.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Backend.Domain.Enums;
using Microsoft.AspNetCore.Identity;

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
    public virtual SubSwapEntity? SubSwap { get; set; } // nullable foreign key structure, ofc it will always be here not null

}
