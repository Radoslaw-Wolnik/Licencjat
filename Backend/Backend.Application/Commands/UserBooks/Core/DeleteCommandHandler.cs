using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;
using Backend.Application.Interfaces.DbReads;

namespace Backend.Application.Commands.UserBooks.Core;
public class DeleteUserBookCommandHandler
    : IRequestHandler<DeleteUserBookCommand, Result>
{
    private readonly IWriteUserBookRepository _bookRepo;
    private readonly IUserBookReadService _bookRead;
    private readonly IImageStorageService _imageStorage;

    public DeleteUserBookCommandHandler(
        IWriteUserBookRepository bookRepo,
        IUserBookReadService bookReadService,
        IImageStorageService storage)
    {
        _bookRepo = bookRepo;
        _bookRead = bookReadService;
        _imageStorage  = storage;
    }

    public async Task<Result> Handle(
        DeleteUserBookCommand request,
        CancellationToken cancellationToken)
    {
        var Id = request.UserBookId;
        var book = await _bookRead.GetByIdAsync(Id, cancellationToken);
        var photoKey = book.CoverPhoto.Link;

        // ask the storage to delete the book
        var deleteResult = await _bookRepo.DeleteAsync(Id, cancellationToken);
        if (deleteResult.IsFailed)
            return Result.Fail(deleteResult.Errors);
        
        // ask the image storage service to delete the photos
        await _imageStorage.DeleteAsync(photoKey, cancellationToken);

        return Result.Ok();
    }
}
