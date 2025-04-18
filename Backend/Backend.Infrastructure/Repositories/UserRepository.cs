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
        // Map domain predicate to infrastructure model in the projection in between
        var projectionPredicate = _mapper.MapExpression<Expression<Func<UserProjection, bool>>>(predicate);
        var entityPredicate = _mapper.MapExpression<Expression<Func<UserEntity, bool>>>(projectionPredicate);
        
        bool exists = await _context.Users
            .AnyAsync(entityPredicate);
        
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

    public async Task<Result<User>> FirstOrDefaultAsync(
        Expression<Func<UserProjection, bool>> predicate
    ){
        try
        {
            Console.WriteLine($"[User repository] passed expression: {predicate}");
            var projectionPredicate = _mapper.MapExpression<Expression<Func<UserProjection, bool>>>(predicate);
            var entityPredicate = _mapper.MapExpression<Expression<Func<UserEntity, bool>>>(projectionPredicate);
            
            var dbUser = await _context.Users
                .FirstOrDefaultAsync(entityPredicate);
                
            return dbUser != null 
                ? _mapper.Map<User>(dbUser)
                // : Result.Fail<User>(new DomainError("User.NotFound", "User not found", ErrorType.NotFound));
                : Result.Fail<User>(UserErrors.NotFound);
        }
        catch (Exception ex)
        {
            return Result.Fail<User>(new DomainError(
                "DatabaseError", 
                ex.Message, 
                ErrorType.StorageError));
        }
    }

    public async Task<Result<User>> GetUserWithIncludes(
        Guid userId, 
        params Expression<Func<UserEntity, object>>[] includes
    ){
        try
        {
            var query = _context.Users.AsQueryable();
            
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            var entity = await query.FirstOrDefaultAsync(u => u.Id == userId);
            // return _mapper.Map<User>(entity);
            return entity != null 
                ? _mapper.Map<User>(entity)
                : Result.Fail<User>(UserErrors.NotFound);
            }
        catch (Exception ex)
        {
            return Result.Fail<User>(new DomainError(
                "DatabaseError", 
                ex.Message, 
                ErrorType.StorageError));
        }
    }


    // public async Task<Result<?>> UpdateAsync(User newUserData) {}

}