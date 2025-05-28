using Backend.Domain.Enums;

namespace Backend.API.DTOs.Swaps.Responses;

public sealed record FeedbackResponse(
    Guid Id,
    Guid SwapId,
    int Stars,
    bool Recommend,
    SwapLength Length,
    SwapConditionBook Condition,
    SwapCommunication Communication
);
