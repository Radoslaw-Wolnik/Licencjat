using Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Linq.Expressions;
using AutoMapper.Extensions.ExpressionMapping;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Common;
using Backend.Application.ReadModels.Users;

namespace Backend.Infrastructure.Services.DbReads;

public class UserReadService : IUserReadService
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public UserReadService(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<bool> ExistsAsync(
        Expression<Func<UserProjection, bool>> predicate,
        CancellationToken cancellationToken = default
    ) {
        return await _db.Users
            .ProjectTo<UserProjection>(_mapper.ConfigurationProvider)
            .AnyAsync(predicate, cancellationToken);
    }

    public async Task<User?> GetByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default
    ) {
        return await _db.Users
            .Where(u => u.Id == userId)
            .ProjectTo<User>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<User?> GetUserWithIncludes(
        Guid userId,
        CancellationToken cancellationToken = default,
        params Expression<Func<UserProjection, object>>[] includes
    ) {
        IQueryable<UserEntity> query = _db.Users.AsNoTracking();

        foreach (var include in includes)
        {
            var entityInclude = _mapper.MapExpression<Expression<Func<UserEntity, object>>>(include);
            query = query.Include(entityInclude);
        }

        var entity = await query.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
        if (entity == null)
            return null;

        return _mapper.Map<User>(entity);
    }

    public async Task<SocialMediaLink> GetSocialMediaByIdAsync(
        Guid SocialMediaId,
        CancellationToken cancellationToken
    ) {
        var entity = await _db.SocialMediaLinks.
            FirstOrDefaultAsync(sml => sml.Id == SocialMediaId, cancellationToken);

        return _mapper.Map<SocialMediaLink>(entity);
    }
}
