using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Feedbacks;

public sealed record CreateFeedbackCommand(
    Guid SwapId,
    Guid UserId,
    int Stars,
    bool Recommend,
    SwapLength Length,
    SwapConditionBook Condition,
    SwapCommunication Communication
    ) : IRequest<Result>; // <Result<Feedback>>  or <Result<Guid>>