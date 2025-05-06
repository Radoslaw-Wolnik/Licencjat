using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using System.Linq.Expressions;
using AutoMapper.Extensions.ExpressionMapping;
using Backend.Application.DTOs;
using Backend.Application.DTOs.Commands.Auth;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;

namespace Backend.Infrastructure.Repositories.Users;

public class AuthUserRepository : IAuthUserRepository // change to read service
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AuthUserRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<LoginUserInfo?> GetLoginInfoAsync(
        Expression<Func<UserProjection,bool>> predicate
    ){
        var entityPredicate = 
            _mapper.MapExpression<Expression<Func<UserEntity, bool>>>(predicate);

        return await _context.Users
            .Where(entityPredicate)
            .Select(u => new LoginUserInfo {
            Id           = u.Id,
            Email        = u.Email,
            UserName     = u.UserName,
            // PasswordHash = u.PasswordHash,
            // …
            })
            .FirstOrDefaultAsync();
    }
}