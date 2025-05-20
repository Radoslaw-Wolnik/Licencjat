using Backend.Application.Interfaces.Repositories;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;
using Backend.Domain.Common;
using Backend.Domain.Enums;

namespace Backend.Application.Commands.UserBooks.Core;
public class UpdateUserBookCoverCommandHandler
    : IRequestHandler<UpdateUserBookCoverCommand, Result<string>>
{
    private readonly IWriteUserBookRepository _bookRepo;
    private readonly IUserBookReadService _bookRead;
    private readonly IImageStorageService _imageStorage;

    public UpdateUserBookCoverCommandHandler(
        IWriteUserBookRepository bookRepo,
        IUserBookReadService bookRead,
        IImageStorageService storage)
    {
        _bookRepo = bookRepo;
        _bookRead = bookRead;
        _imageStorage  = storage;
    }

    public async Task<Result<string>> Handle(
        UpdateUserBookCoverCommand request,
        CancellationToken cancellationToken)
    {
        // get book
        var book = await _bookRead.GetByIdAsync(request.UserBookId, cancellationToken);
        
        if (book == null)
            return Result.Fail(DomainErrorFactory.NotFound("UserBook", request.UserBookId));

        // ask the storage service for objectKey
        var objectKey = _imageStorage.GenerateObjectKey(
            StorageDestination.GeneralBooks,
            book.Id,
            request.CoverFileName);

        // build your Photo metadata with the objectKey
        var photo = new Photo(objectKey);

        // change the photo in userbook
        book.UpdateCover(photo);

        // save the generalBook scalars
        var saveResult = await _bookRepo.UpdateScalarsAsync(book, cancellationToken);
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors);
        
        // ask the storage service for presigned URL
        var uploadUrl = await _imageStorage.GenerateUploadUrlAsync(objectKey);

        return Result.Ok((
            uploadUrl));
    }
}
