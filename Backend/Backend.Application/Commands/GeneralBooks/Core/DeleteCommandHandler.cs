using Backend.Application.Interfaces.Repositories;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;

namespace Backend.Application.Commands.GeneralBooks.Core;
public class DeleteGeneralBookCommandHandler
    : IRequestHandler<DeleteGeneralBookCommand, Result>
{
    private readonly IWriteGeneralBookRepository _bookRepo;
    private readonly IImageStorageService _imageStorage;

    public DeleteGeneralBookCommandHandler(
        IWriteGeneralBookRepository bookRepo,
        IImageStorageService storage)
    {
        _bookRepo = bookRepo;
        _imageStorage  = storage;
    }

    public async Task<Result> Handle(
        DeleteGeneralBookCommand request,
        CancellationToken cancellationToken)
    {
        var Id = request.GeneralBookId;

        // ask the storage to delete the book
        var deleteResult = await _bookRepo.DeleteAsync(Id, cancellationToken);
        if (deleteResult.IsFailed)
            return Result.Fail(deleteResult.Errors);
        
        // ask the image storage service to delete thse photos
        await _imageStorage.DeleteAsync(request.PhotoKey, cancellationToken);

        return Result.Ok();
    }
}
