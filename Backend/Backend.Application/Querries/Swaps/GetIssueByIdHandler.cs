using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Swaps;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Swaps;

public class GetIssueByIdHandler
    : IRequestHandler<GetIssueByIdQuery, Result<IssueReadModel>>
{
    private readonly ISwapQueryService _swapQuery;

    public GetIssueByIdHandler(
        ISwapQueryService swapQueryService)
    {
        _swapQuery = swapQueryService;
    }

    public async Task<Result<IssueReadModel>> Handle(
        GetIssueByIdQuery request,
        CancellationToken cancellationToken)
    {
        var issue = await _swapQuery.GetIssueByIdAsync(
            request.IssueId, cancellationToken);

        if (issue is null)
        {
            return Result.Fail(DomainErrorFactory.NotFound("Issue", request.IssueId));
        }

        return Result.Ok(issue);
    }

}