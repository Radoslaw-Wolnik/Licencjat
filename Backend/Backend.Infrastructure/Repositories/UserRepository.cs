// Backend.Infrastructure/Repositories/UserRepository.cs
using Backend.Domain.Entities;
using Backend.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace Backend.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    
    public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    
    public async Task<User> AddAsync(User user, string password)
    {
        // Map Domain User to ApplicationUser (adjust mapping as needed)
        var appUser = new ApplicationUser
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            BirthDate = user.BirthDate,
            // Additional mapping if necessary
        };

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
        // Example retrieval and mapping
        var appUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (appUser == null)
            return null;

        if (string.IsNullOrEmpty(appUser.Email) || string.IsNullOrEmpty(appUser.PasswordHash))
        {
            throw new InvalidOperationException("User data is incomplete in the databse lol you must be a really special");
        }

        // Map ApplicationUser back to Domain User (this mapping could be handled by a dedicated mapper)
        var domainUser = new User(
            appUser.Email,
            appUser.FirstName,
            appUser.LastName,
            appUser.BirthDate);
        return domainUser;
    }
    
    // Implement additional methods as required
}

