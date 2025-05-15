using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using FluentResults;
using Backend.Infrastructure.Extensions;

namespace Backend.Infrastructure.Repositories.Swaps;

public class SwapIssueRepository : ISwapIssueRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public SwapIssueRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db     = db;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> AddAsync(Issue issue, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<IssueEntity>(issue);
        _db.Issues.Add(entity);
        
        var result = await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to add Issue");
        return result.IsSuccess
            ? Result.Ok(entity.Id)
            : Result.Fail<Guid>(result.Errors);
    }

    public async Task<Result> UpdateAsync(Issue issue, CancellationToken cancellationToken)
    {
        var existing = await _db.Issues.FindAsync(issue.Id);
        _mapper.Map(existing, issue);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to update Issue");
    }

    public async Task<Result> RemoveAsync(Guid issueId, CancellationToken cancellationToken)
    {
        var existing = await _db.Issues.FindAsync(issueId);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("Issue", issueId));

        _db.Issues.Remove(existing);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to delete issue");
    }

    
    
}
