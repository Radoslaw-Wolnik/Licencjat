using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;

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

    public async Task<Issue> GetByIdAsync(Guid subSwapId)
    {
        var entities = await _db.Issues
            .AsNoTracking()
            .Where(x => x.SubSwapId == subSwapId)
            .FirstOrDefaultAsync();
        return _mapper.Map<Issue>(entities);
    }

    public async Task AddAsync(Issue issue)
    {
        var entity = _mapper.Map<IssueEntity>(issue);
        _db.Issues.Add(entity);
        await _db.SaveChangesAsync();
    }

    public async Task RemoveAsync(Guid issueId)
    {
        var existing = await _db.Issues.FindAsync(issueId);
        if (existing is null)
            throw new KeyNotFoundException($"Issue with Id = {issueId} was not found.");

        _db.Issues.Remove(existing);
    }

    public async Task UpdateAsync(Issue issue)
    {
        var existing = await _db.Issues.FindAsync(issue.Id);
        _mapper.Map(existing, issue);
        await _db.SaveChangesAsync();
    }
    
}
