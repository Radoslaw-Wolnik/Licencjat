using Backend.Domain.Entities;

namespace Backend.Application.Interfaces;

public interface IUserService
{
    Task RegisterUserAsync(string email, string username, string password, string firstName, string lastName, DateTime birthDate);
    Task LoginUserAsync(string email, string password, bool rememberMe);
    Task<User?> GetByIdAsync(Guid id);
}
