// Backend.Infrastructure/Repositories/UserRepository.cs
using Backend.Domain.Entities;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using Backend.Domain.Errors;
using Backend.Application.Interfaces;

namespace Backend.Infrastructure.Repositories.GeneralBooks;

public interface IWriteGeneralBookRepository
{
    Task AddAsync(GeneralBook user);
    Task UpdateAsync(GeneralBook book);
    Task DeleteAsync(Guid bookId);
}



public class WriteGeneralBookRepository : IWriteGeneralBookRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public WriteGeneralBookRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task AddAsync(GeneralBook book)
    {
        var dbBook = _mapper.Map<GeneralBookEntity>(book);
        _context.GeneralBooks.Add(dbBook);

        await _context.SaveChangesAsync();
        // return Result.Ok(dbBook.Id);

    }

    public async Task UpdateAsync(GeneralBook book)
    {
        var existing = await _context.GeneralBooks.FindAsync(book.Id);
        _mapper.Map(book, existing);
        await _context.SaveChangesAsync();

    }

    public async Task DeleteAsync(Guid bookId)
    {
        var existing = await _context.GeneralBooks.FindAsync(bookId);
        if (existing is null)
            throw new KeyNotFoundException($"GeneralBook with Id = {bookId} was not found.");
            
        _context.GeneralBooks.Remove(existing);    
    }
}