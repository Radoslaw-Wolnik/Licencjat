using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Issues;

public sealed record DeleteCommand(
    Guid IssueId
    ) : IRequest<Result>;