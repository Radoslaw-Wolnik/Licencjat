using Backend.Domain.Entities;

namespace Backend.Application.Interfaces.Repositories;

public interface IWriteGeneralBookRepository
{
    Task AddAsync(GeneralBook user);
    Task UpdateAsync(GeneralBook book);
    Task DeleteAsync(Guid bookId);
}
