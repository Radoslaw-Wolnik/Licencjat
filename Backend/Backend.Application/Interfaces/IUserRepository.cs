// Application/Interfaces/IUserRepository.cs
using Backend.Domain.Entities;

namespace Backend.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User> AddAsync(User user, string password);
        // Define other necessary methods (e.g., Update, Delete, etc.)
    }
}
