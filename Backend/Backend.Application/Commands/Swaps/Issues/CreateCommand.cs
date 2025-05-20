using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Issues;

public sealed record CreateIssueCommand(
    Guid SwapId,
    Guid UserId,
    string Description
    ) : IRequest<Result>; // <Result<Issue>> or <Result<Guid>>