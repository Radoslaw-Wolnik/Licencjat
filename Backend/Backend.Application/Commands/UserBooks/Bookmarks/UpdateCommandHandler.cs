using FluentResults;
using MediatR;
using Backend.Application.Interfaces.DbReads;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Common;
using Backend.Domain.Errors;

namespace Backend.Application.Commands.UserBooks.Bookmarks;

public class UpdateBookmarkCommandHandler
    : IRequestHandler<UpdateBookmarkCommand, Result<Bookmark>>
{
    private readonly IWriteUserBookRepository _bookRepo;
    private readonly IUserBookReadService _bookRead;

    public UpdateBookmarkCommandHandler(
        IWriteUserBookRepository bookRepo,
        IUserBookReadService bookReadService)
    {
        _bookRepo = bookRepo;
        _bookRead = bookReadService;
    }

    public async Task<Result<Bookmark>> Handle(
        UpdateBookmarkCommand request,
        CancellationToken cancellationToken)
    {
        // load the exsisting - previous review
        var existing = await _bookRead.GetBookmarkByIdAsync(request.BookmarkId, cancellationToken);
        if (existing == null)
            return Result.Fail("Bookmark not found");

        var old = existing;

        // Merge in the possible differences
        var newColour = request.Colour ?? old.Colour;
        var newPage = request.Page ?? old.Page;
        var newDescription = request.Description ?? old.Description;

        var createResult = Bookmark.Create(
            id: old.Id,
            userBookId: old.UserBookId,
            colour: newColour,
            page: newPage,
            description: newDescription
        );
        if (createResult.IsFailed)
            return Result.Fail(createResult.Errors);

        var updated = createResult.Value;

        // persistance save
        return await _bookRepo.UpdateBookmarkAsync(updated, cancellationToken);
    }
}
