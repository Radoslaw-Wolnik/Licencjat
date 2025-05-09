// Backend.Infrastructure/Repositories/UserRepository.cs
using Backend.Domain.Entities;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using Backend.Infrastructure.Extensions;
using FluentResults;

namespace Backend.Infrastructure.Repositories.GeneralBooks;

public class WriteGeneralBookRepository : IWriteGeneralBookRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public WriteGeneralBookRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<Result<Guid>> AddAsync(GeneralBook book, CancellationToken cancellationToken)
    {
        var dbBook = _mapper.Map<GeneralBookEntity>(book);
        _context.GeneralBooks.Add(dbBook);

        // await _context.SaveChangesAsync(cancellationToken);
        var result = await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to add book");
        
        return result.IsSuccess
            ? Result.Ok(dbBook.Id)
            : Result.Fail<Guid>(result.Errors);
    }

    public async Task<Result> UpdateAsync(GeneralBook book, CancellationToken cancellationToken)
    {
        var existing = await _context.GeneralBooks.FindAsync(book.Id, cancellationToken);
        _mapper.Map(book, existing);
        // await _context.SaveChangesAsync(cancellationToken);
        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to update book");
    }

    public async Task<Result> DeleteAsync(Guid bookId, CancellationToken cancellationToken)
    {
        var existing = await _context.GeneralBooks.FindAsync(bookId, cancellationToken);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("GeneralBook", bookId));
            
        _context.GeneralBooks.Remove(existing);

        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to delete book");
    }
}