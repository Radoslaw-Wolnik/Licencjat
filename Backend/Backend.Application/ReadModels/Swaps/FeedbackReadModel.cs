using Backend.Domain.Enums;

namespace Backend.Application.ReadModels.Swaps;

public sealed record FeedbackReadModel(
    Guid Id,
    Guid SwapId,
    int Stars,
    bool Recommend,
    SwapLength Length,
    SwapConditionBook Condition,
    SwapCommunication Communication
);