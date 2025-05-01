using Backend.Domain.Entities;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using Backend.Domain.Errors;
using Backend.Application.Interfaces;

namespace Backend.Infrastructure.Repositories.UserBooks;

public interface IWriteUserBookRepository
{
    Task AddAsync(UserBook book);

    Task UpdateAsync(UserBook book);
    Task DeleteAsync(Guid bookId);
}



public class WriteUserBookRepository : IWriteUserBookRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public WriteUserBookRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task AddAsync(UserBook book)
    {
        var dbBook = _mapper.Map<UserBookEntity>(book);
        _context.UserBooks.Add(dbBook);
        await _context.SaveChangesAsync();
        // return dbBook.Id;
    }

    public async Task UpdateAsync(UserBook book)
    {
        var existing = await _context.UserBooks.FindAsync(book.Id);
        _mapper.Map(book, existing);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid bookId)
    {
        var existing = await _context.UserBooks.FindAsync(bookId);
        if (existing is null)
            throw new KeyNotFoundException($"UserBook with Id = {bookId} was not found.");

        _context.UserBooks.Remove(existing);
        await _context.SaveChangesAsync();  
    }
}