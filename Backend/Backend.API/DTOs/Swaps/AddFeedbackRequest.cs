using System.ComponentModel.DataAnnotations;
using Backend.Domain.Enums;

namespace Backend.API.DTOs.Swaps;

public sealed record AddFeedbackRequest(
    [Required] Guid SwapId,
    [Range(1, 5)] int Stars,
    bool Recommend,
    SwapLength Length,
    SwapConditionBook Condition,
    SwapCommunication Communication
);
