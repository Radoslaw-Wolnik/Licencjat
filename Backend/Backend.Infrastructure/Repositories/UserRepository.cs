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

    public async Task<Result<bool>> ExistsAsync(Expression<Func<User, bool>> predicate)
    {
        // Map domain predicate to infrastructure model
        var appUsrPredicate = _mapper.MapExpression<Expression<Func<ApplicationUser, bool>>>(predicate);
        
        bool exists = await _context.Users
            .AnyAsync(appUsrPredicate);
        
        return Result.Ok(exists);
    }

    public async Task<Result<Guid>> AddAsync(User user)
    {
        var appUser = _mapper.Map<ApplicationUser>(user);
        _context.Users.Add(appUser);

        try
        {    
            await _context.SaveChangesAsync();
            return Result.Ok(appUser.Id);
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
        var appUser = await _context.Users.FindAsync(id);
        return appUser is null 
            ? Result.Fail<User>(new DomainError("User.NotFound", "User not found", ErrorType.NotFound)) 
            : Result.Ok(_mapper.Map<User>(appUser));
    }

    public async Task<Result<User>> FirstOrDefaultAsync(Expression<Func<User, bool>> predicate)
    {
        try
        {
            var appUserPredicate = _mapper.MapExpression<Expression<Func<ApplicationUser, bool>>>(predicate);
            
            var appUser = await _context.Users
                .FirstOrDefaultAsync(appUserPredicate);
                
            return appUser != null 
                ? _mapper.Map<User>(appUser)
                : Result.Fail<User>(new DomainError("User.NotFound", "User not found", ErrorType.NotFound));
        }
        catch (Exception ex)
        {
            return Result.Fail<User>(new DomainError(
                "DatabaseError", 
                ex.Message, 
                ErrorType.StorageError));
        }
    }
}