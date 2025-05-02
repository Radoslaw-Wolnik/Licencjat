using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Linq.Expressions;
using AutoMapper.Extensions.ExpressionMapping;
using Backend.Application.DTOs;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.DbReads;

namespace Backend.Infrastructure.Services.DbReads;

public class UserReadService : IUserReadService
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UserReadService(ApplicationDbContext db, IMapper mapper)
    {
        _db     = db;
        _mapper = mapper;
    }

    public async Task<bool> ExistsAsync(Expression<Func<UserProjection, bool>> predicate)
    {
        return await _db.Users
            .ProjectTo<UserProjection>(_mapper.ConfigurationProvider)
            .AnyAsync(predicate);
    }

    public async Task<User?> GetByIdAsync(Guid userId)
    {
        return await _db.Users
            .Where(u => u.Id == userId)
            .ProjectTo<User>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();
    }

    public async Task<User?> GetByAsync(Expression<Func<UserProjection, bool>> predicate)
    {
        var entityPredicate = 
            _mapper.MapExpression<Expression<Func<UserEntity, bool>>>(predicate);
        var entity = await _db.Users
            .FirstOrDefaultAsync(entityPredicate);
        
        return _mapper.Map<User>(entity);
    }

    public async Task<User?> GetUserWithIncludes(
        Guid userId, 
        params Expression<Func<UserProjection, object>>[] includes)
    {
        IQueryable<UserEntity> query = _db.Users.AsNoTracking();

        foreach (var include in includes)
        {
            var entityInclude = _mapper.MapExpression<Expression<Func<UserEntity, object>>>(include);
            query = query.Include(entityInclude);
        }

        var entity = await query.FirstOrDefaultAsync(u => u.Id == userId);
        if (entity == null)
            return null;

        return _mapper.Map<User>(entity);
    }
}
