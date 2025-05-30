using Backend.Application.ReadModels.Swaps;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Swaps;

public sealed record GetIssueByIdQuery(
    Guid IssueId
) : IRequest<Result<IssueReadModel>>;