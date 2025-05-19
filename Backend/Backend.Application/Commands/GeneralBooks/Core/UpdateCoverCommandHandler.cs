using Backend.Application.Interfaces.Repositories;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;
using Backend.Domain.Common;
using Backend.Domain.Enums;

namespace Backend.Application.Commands.GeneralBooks.Core;
public class UpdateGeneralBookCoverCommandHandler
    : IRequestHandler<UpdateGeneralBookCoverCommand, Result<string>>
{
    private readonly IWriteGeneralBookRepository _bookRepo;
    private readonly IGeneralBookReadService _bookRead;
    private readonly IImageStorageService _imageStorage;

    public UpdateGeneralBookCoverCommandHandler(
        IWriteGeneralBookRepository bookRepo,
        IGeneralBookReadService bookRead,
        IImageStorageService storage)
    {
        _bookRepo = bookRepo;
        _bookRead = bookRead;
        _imageStorage  = storage;
    }

    public async Task<Result<string>> Handle(
        UpdateGeneralBookCoverCommand request,
        CancellationToken cancellationToken)
    {
        // get book
        var book = await _bookRead.GetByIdAsync(request.BookId, cancellationToken);
        
        if (book == null)
            return Result.Fail(DomainErrorFactory.NotFound("GeneralBook", request.BookId));

        // ask the storage service for objectKey
        var objectKey = _imageStorage.GenerateObjectKey(
            StorageDestination.GeneralBooks,
            book.Id,
            request.CoverFileName);

        // build your Photo metadata with the objectKey
        var photo = new Photo(objectKey);

        // change the photo in generalbook
        var updateResult = book.UpdateCoverPhoto(photo);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors);

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
