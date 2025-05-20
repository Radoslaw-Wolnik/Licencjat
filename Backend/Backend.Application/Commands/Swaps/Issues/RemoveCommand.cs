using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Issues;

public sealed record RemoveIssueCommand(
    Guid IssueId
    ) : IRequest<Result>;