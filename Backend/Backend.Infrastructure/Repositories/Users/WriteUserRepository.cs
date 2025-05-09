using Backend.Domain.Entities;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using FluentResults;
using Backend.Infrastructure.Extensions;

namespace Backend.Infrastructure.Repositories.Users;

public class WriteUserRepository : IWriteUserRepository
{
    private readonly ApplicationDbContext _db;
    private readonly IMapper _mapper;

    public WriteUserRepository(ApplicationDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> AddAsync(User user, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<UserEntity>(user);
        _db.Users.Add(entity);
        var result = await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to save user");
        
        return result.IsSuccess
            ? Result.Ok(entity.Id)
            : Result.Fail<Guid>(result.Errors);
        // user.SetId(entity.Id); 
    }

    public async Task<Result> UpdateAsync(User user, CancellationToken cancellationToken)
    {
        var existing = await _db.Users.FindAsync(user.Id);
        _mapper.Map(user, existing);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to update User");
    }

    public async Task<Result> DeleteAsync(Guid userId, CancellationToken cancellationToken)
    {
        var existing = await _db.Users.FindAsync(userId);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("User", userId));

        _db.Users.Remove(existing);
        return await _db.SaveChangesWithResultAsync(cancellationToken, "Failed to delete User");
    }
}
