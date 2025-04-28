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

// authentication info 
public interface IAuthUserRepository
{
    Task<Result<LoginUserInfo>> GetLoginInfoAsync(Expression<Func<UserProjection,bool>> predicate);
    
}



public class AuthUserRepository : IAuthUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AuthUserRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
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

}