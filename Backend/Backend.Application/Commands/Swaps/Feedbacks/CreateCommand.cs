using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Feedbacks;

public sealed record CreateCommand(
    Guid SubSwapId, // or Swap Id
    Guid UserId,
    int Stars,
    bool Recommend,
    SwapLength Length,
    SwapConditionBook Condition,
    SwapCommunication Communication
    ) : IRequest<Result>; // <Result<Feedback>> 