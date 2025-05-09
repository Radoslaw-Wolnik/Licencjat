using Backend.Domain.Entities;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using AutoMapper;
using Backend.Domain.Errors;
using Backend.Application.Interfaces.Repositories;
using FluentResults;
using Backend.Infrastructure.Extensions;
using System.Security.Cryptography;

namespace Backend.Infrastructure.Repositories.UserBooks;

public class WriteUserBookRepository : IWriteUserBookRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public WriteUserBookRepository(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<Result<Guid>> AddAsync(UserBook book, CancellationToken cancellationToken)
    {
        var dbBook = _mapper.Map<UserBookEntity>(book);
        _context.UserBooks.Add(dbBook);
        var result = await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to add UserBook");
        return result.IsSuccess
            ? Result.Ok(dbBook.Id)
            : Result.Fail<Guid>(result.Errors);
    }

    public async Task<Result> UpdateAsync(UserBook book, CancellationToken cancellationToken)
    {
        var existing = await _context.UserBooks.FindAsync(book.Id);
        _mapper.Map(book, existing);
        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to update UserBook");
    }

    public async Task<Result> DeleteAsync(Guid bookId, CancellationToken cancellationToken)
    {
        var existing = await _context.UserBooks.FindAsync(bookId);
        if (existing is null)
            return Result.Fail(DomainErrorFactory.NotFound("UserBook", bookId));

        _context.UserBooks.Remove(existing);
        return await _context.SaveChangesWithResultAsync(cancellationToken, "Failed to delete UserBook");  
    }
}