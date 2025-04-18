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

namespace Backend.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UserRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<bool>> ExistsAsync(Expression<Func<UserProjection, bool>> predicate)
    {
        var exists = await _context.Users
            .ProjectTo<UserProjection>(_mapper.ConfigurationProvider)
            .AnyAsync(predicate);
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
        params Expression<Func<UserEntity, object>>[] includes
    ){
        try
        {
            IQueryable<UserEntity> query = _context.Users;
            foreach (var inc in includes)
                query = query.Include(inc);

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

    public async Task<Result<LoginUserInfo>> GetLoginInfoAsync(
        Expression<Func<UserProjection,bool>> predicate
    ){
        var entityPredicate = 
            _mapper.MapExpression<Expression<Func<UserEntity, bool>>>(predicate);

        var info = await _context.Users
            .Where(entityPredicate)
            .Select(u => new LoginUserInfo {
            Id           = u.Id,
            Email        = u.Email,
            UserName     = u.UserName,
            // PasswordHash = u.PasswordHash,
            // …
            })
            .FirstOrDefaultAsync();

        return info is null
            ? Result.Fail<LoginUserInfo>(AuthErrors.InvalidCredentials)
            : Result.Ok(info);
    }


    // public async Task<Result<?>> UpdateAsync(User newUserData) {}

}