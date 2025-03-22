// Backend.Infrastructure/Repositories/UserRepository.cs
using Backend.Domain.Entities;
using Backend.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using AutoMapper;

namespace Backend.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IMapper _mapper;
    
    public UserRepository(
        ApplicationDbContext context, 
        UserManager<ApplicationUser> 
        userManager, IMapper mapper)
    {
        _context = context;
        _userManager = userManager;
        _mapper = mapper;
    }
    
    public async Task<User> AddAsync(User user, string password)
    {

        var appUser = _mapper.Map<ApplicationUser>(user);

        // _context.Users.Add(appUser);
        // await _context.SaveChangesAsync();
        var result = await _userManager.CreateAsync(appUser, password);
        if (!result.Succeeded)
        {
            // Handle errors (e.g., throw an exception or return a custom result)
            throw new ApplicationException("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        
        return user;
    }
    
    public async Task<User?> GetByIdAsync(Guid id)
    {
        var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (appUser == null)
            return null;

        if (string.IsNullOrEmpty(appUser.Email) || string.IsNullOrEmpty(appUser.PasswordHash))
        {
            throw new InvalidOperationException("User data is incomplete in the databse lol you must be a really special");
        }

        var domainUser = _mapper.Map<User>(appUser);
        return domainUser;
    }
    
}

