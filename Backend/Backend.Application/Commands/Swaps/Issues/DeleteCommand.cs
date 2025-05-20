using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Issues;

public sealed record DeleteIssueCommand(
    Guid IssueId
    ) : IRequest<Result>;