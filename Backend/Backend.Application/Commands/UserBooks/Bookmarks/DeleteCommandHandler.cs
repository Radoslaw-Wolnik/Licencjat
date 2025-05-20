using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using Backend.Domain.Errors;

namespace Backend.Application.Commands.UserBooks.Bookmarks;

public class DeleteBookmarkCommandHandler
    : IRequestHandler<DeleteBookmarkCommand, Result>
{
    private readonly IWriteUserBookRepository _bookRepo;

    public DeleteBookmarkCommandHandler(
        IWriteUserBookRepository bookRepo)
    {
        _bookRepo = bookRepo;
    }

    public async Task<Result> Handle(
        DeleteBookmarkCommand request,
        CancellationToken cancellationToken)
    {
        return await _bookRepo.RemoveBookmarkAsync(request.BookmarkId, cancellationToken);
    }
}
