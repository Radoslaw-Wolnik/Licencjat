using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;
using Backend.Domain.Common;
using Backend.Domain.Enums;

namespace Backend.Application.Commands.GeneralBooks.Core;
public class CreateGeneralBookCommandHandler
    : IRequestHandler<CreateGeneralBookCommand, Result<(Guid, string)>>
{
    private readonly IWriteGeneralBookRepository _bookRepo;
    private readonly IImageStorageService _imageStorage;

    public CreateGeneralBookCommandHandler(
        IWriteGeneralBookRepository bookRepo,
        IImageStorageService storage)
    {
        _bookRepo = bookRepo;
        _imageStorage  = storage;
    }

    public async Task<Result<(Guid, string)>> Handle(
        CreateGeneralBookCommand request,
        CancellationToken cancellationToken)
    {
        // Convert/validate the language code
        var langResult = LanguageCode.Create(request.OryginalLanguage);
        if (langResult.IsFailed)
            return Result.Fail(langResult.Errors);
        
        var bookId = Guid.NewGuid(); 

        // ask the storage service for objectKey
        var objectKey = _imageStorage.GenerateObjectKey(
            StorageDestination.GeneralBooks,
            bookId,
            request.CoverFileName);

        // build your Photo metadata with the objectKey
        var photo = new Photo(objectKey);

        // Call the domain factory
        var bookResult = GeneralBook.Create(
            bookId,
            request.Title,
            request.Author,
            request.Published,
            langResult.Value,
            photo);

        if (bookResult.IsFailed)
            return Result.Fail(bookResult.Errors);

        var book = bookResult.Value;

        // ask the storage to save the domain entity
        var saveResult = await _bookRepo.AddAsync(book, cancellationToken);
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors);
        
        // ask the storage service for presigned URL
        var uploadUrl = await _imageStorage.GenerateUploadUrlAsync(objectKey);

        return Result.Ok((
            bookId,
            uploadUrl));
    }
}
