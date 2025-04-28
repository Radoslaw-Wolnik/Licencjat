// Backend.Infrastructure/Repositories/UserRepository.cs
using Backend.Domain.Entities;
using Backend.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using FluentResults;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using System.Linq.Expressions;
using Backend.Domain.Common;
using AutoMapper.Extensions.ExpressionMapping;
using Backend.Domain.Errors;
using Backend.Infrastructure.Mapping;
using Backend.Application.DTOs;
using Backend.Application.DTOs.Auth;

namespace Backend.Infrastructure.Repositories.Users;

// Core user (scalars + existence)
public interface ICoreUserRepository
{
    Task<Result<bool>> ExistsAsync(Expression<Func<UserProjection, bool>> predicate);
    Task<Result<Guid>> AddAsync(User user);
    Task<Result<User>> GetByIdAsync(Guid id);
    Task<Result<User>> GetByAsync(Expression<Func<UserProjection, bool>> predicate);
    Task<Result<User>> GetUserWithIncludes(Guid userId, 
        params Expression<Func<UserProjection, object>>[] includes);

    Task<Result>  UpdateAsync(User user);
    Task<Result>  DeleteAsync(Guid id);
}



public class CoreUserRepository : ICoreUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CoreUserRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<bool>> ExistsAsync(Expression<Func<UserProjection, bool>> predicate)
    {
        var entityPredicate = 
                _mapper.MapExpression<Expression<Func<UserEntity, bool>>>(predicate);
        var exists = await _context.Users
            .AnyAsync(entityPredicate);
        // var exists = await _context.Users
        //     .ProjectTo<UserProjection>(_mapper.ConfigurationProvider)
        //     .AnyAsync(predicate);
        return Result.Ok(exists);
    }

    public async Task<Result<Guid>> AddAsync(User user)
    {
        var dbUser = _mapper.Map<UserEntity>(user);
        _context.Users.Add(dbUser);

        try
        {    
            await _context.SaveChangesAsync();
            return Result.Ok(dbUser.Id);
        }
        catch (DbUpdateException ex)
        {
            // Handle DB errors (e.g., unique constraint violations)
            return Result.Fail<Guid>(new DomainError(
                "DatabaseError", 
                ex.InnerException?.Message ?? ex.Message, 
                ErrorType.Conflict
        ));
        }
    }

    public async Task<Result<User>> GetByIdAsync(Guid id)
    {
        var dbUser = await _context.Users.FindAsync(id);
        return dbUser is null 
            // ? Result.Fail<User>(new DomainError("User.NotFound", "User not found", ErrorType.NotFound)) 
            ? Result.Fail<User>(UserErrors.NotFound)
            : Result.Ok(_mapper.Map<User>(dbUser));
    }

    public async Task<Result<User>> GetByAsync(
        Expression<Func<UserProjection, bool>> predicate
    ){
        try
        {
            var entityPredicate = 
                _mapper.MapExpression<Expression<Func<UserEntity, bool>>>(predicate);

            var entity = await _context.Users
                .FirstOrDefaultAsync(entityPredicate);
            
            if (entity is null)
                return Result.Fail<User>(UserErrors.NotFound);

            // Automapper will then run your ConstructUsing(src=>MapToDomain(src))
            var user = _mapper.Map<User>(entity);
            return Result.Ok(user);
        }
        catch (Exception ex)
        {
            return Result.Fail<User>
                (new DomainError("DatabaseError", ex.Message, ErrorType.StorageError));
        }
    }

    public async Task<Result<User>> GetUserWithIncludes(
        Guid userId, 
        params Expression<Func<UserProjection, object>>[] includes
    ){
        try
        {
            IQueryable<UserEntity> query = _context.Users;
            foreach (var inc in includes){
                var entityInclude = 
                    _mapper.MapExpression<Expression<Func<UserEntity, bool>>>(inc); // not sure if this will work
                query = query.Include(entityInclude);
            }

            var entity = await query.FirstOrDefaultAsync(u => u.Id == userId);
            if (entity is null)
                return Result.Fail<User>(UserErrors.NotFound);

            return Result.Ok(_mapper.Map<User>(entity));
        }
        catch (Exception ex)
        {
            return Result.Fail<User>(
                new DomainError("DatabaseError", ex.Message, ErrorType.StorageError)
            );
        }
    }

    public async Task<Result> UpdateAsync(User user)
    {
        var existing = await _context.Users.FindAsync(user.Id);
        if (existing == null)
            return Result.Fail(UserErrors.NotFound);

        _mapper.Map(user, existing);

        try
        {    
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // e.g. row-version mismatch
            return Result.Fail(
                new DomainError("ConcurrencyError", ex.Message, ErrorType.Conflict)
            );
        }
        catch (DbUpdateException ex)
        {
            return Result.Fail(
                new DomainError("DatabaseError", ex.InnerException?.Message ?? ex.Message, ErrorType.StorageError)
            );
        }
    }

    public async Task<Result>  DeleteAsync(Guid id)
    {
        var dbUser = await _context.Users.FindAsync(id);
        if (dbUser == null)
            return Result.Fail(UserErrors.NotFound); 

        _context.Users.Remove(dbUser);
        try
        {
            await _context.SaveChangesAsync();
            return Result.Ok();
        }
        catch (DbUpdateException ex)
        {
            // mby more in future for special-cases foreign‐key / constraint errors
            return Result.Fail(
                new DomainError("DatabaseError", ex.InnerException?.Message ?? ex.Message, ErrorType.StorageError)
            );
        }
    }

}