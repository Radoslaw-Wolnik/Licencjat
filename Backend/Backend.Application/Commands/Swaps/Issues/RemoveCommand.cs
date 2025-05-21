using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Issues;

public sealed record RemoveIssueCommand(
    Guid UserId,
    Guid SwapId,
    Guid IssueId,
    string ResolutionDetails
    ) : IRequest<Result>;