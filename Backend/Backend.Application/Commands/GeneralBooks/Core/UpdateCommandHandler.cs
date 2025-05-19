using Backend.Application.Interfaces.Repositories;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Domain.Common;

namespace Backend.Application.Commands.GeneralBooks.Core;
public class UpdateGeneralBookCommandHandler
    : IRequestHandler<UpdateGeneralBookCommand, Result<GeneralBook>>
{
    private readonly IWriteGeneralBookRepository _bookRepo;
    private readonly IGeneralBookReadService _bookRead;

    public UpdateGeneralBookCommandHandler(
        IWriteGeneralBookRepository bookRepo,
        IGeneralBookReadService bookRead)
    {
        _bookRepo = bookRepo;
        _bookRead = bookRead;
    }

    public async Task<Result<GeneralBook>> Handle(
        UpdateGeneralBookCommand request,
        CancellationToken cancellationToken)
    {
        // get book
        var book = await _bookRead.GetByIdAsync(request.BookId, cancellationToken);
        
        if (book == null)
            return Result.Fail(DomainErrorFactory.NotFound("GeneralBook", request.BookId));

        // Convert/validate the language code
        var langResult = request.OryginalLanguage == null ? null : LanguageCode.Create(request.OryginalLanguage);
        if (langResult != null && langResult.IsFailed)
            return Result.Fail(langResult?.Errors);

        var language = langResult?.Value;

        var updateResult = book.UpdateScalarValues(request.Title, request.Author, request.Published, language);
        if (updateResult.IsFailed)
            return Result.Fail(updateResult.Errors);
        
        // update genres of the book - remove old and add new ones
        if (request.NewBookGenres != null){
            var tempRes = book.ReplaceGenres(request.NewBookGenres);
            if (tempRes.IsFailed)
                return Result.Fail(tempRes.Errors);
        }
        
        // and persistance save
        // ask the storage to save the domain entity
        var saveResult = await _bookRepo.UpdateScalarsAsync(book, cancellationToken);
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors);

        return Result.Ok(book);
    }
}