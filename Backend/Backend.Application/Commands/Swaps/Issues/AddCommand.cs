using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Issues;

public sealed record AddIssueCommand(
    Guid SwapId,
    Guid UserId,
    string Description
    ) : IRequest<Result<Guid>>;