using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using Backend.Domain.Common;
using Backend.Domain.Enums;


namespace Backend.Application.Commands.UserBooks.Bookmarks;
public class CreateBookmarkCommandHandler
    : IRequestHandler<CreateBookmarkCommand, Result<Bookmark>>
{
    private readonly IWriteUserBookRepository _bookRepo;

    public CreateBookmarkCommandHandler(
        IWriteUserBookRepository bookRepo)
    {
        _bookRepo = bookRepo;
    }

    public async Task<Result<Bookmark>> Handle(
        CreateBookmarkCommand request,
        CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        // create new review
        var bookmarkResult = Bookmark.Create(id, request.UserBookId, request.Colour, request.Page, request.Description);
        if (bookmarkResult.IsFailed)
            return Result.Fail(bookmarkResult.Errors);
        
        // saev via repository root
        var persistanceResult = await _bookRepo.AddBookmarkAsync(bookmarkResult.Value, cancellationToken);
        if(persistanceResult.IsFailed)
            return Result.Fail(persistanceResult.Errors);

        return Result.Ok(bookmarkResult.Value);
    }
}
