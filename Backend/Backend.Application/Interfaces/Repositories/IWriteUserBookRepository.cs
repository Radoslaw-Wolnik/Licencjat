using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.Repositories;

public interface IWriteUserBookRepository
{
    Task AddAsync(UserBook book);

    Task UpdateAsync(UserBook book);
    Task DeleteAsync(Guid bookId);
}