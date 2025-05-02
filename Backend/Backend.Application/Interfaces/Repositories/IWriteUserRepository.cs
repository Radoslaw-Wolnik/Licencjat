using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.Repositories;

public interface IWriteUserRepository
{
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(Guid id);
}