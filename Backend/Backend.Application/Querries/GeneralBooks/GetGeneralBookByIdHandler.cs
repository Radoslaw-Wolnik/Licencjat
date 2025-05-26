using Backend.Application.Interfaces;
using Backend.Application.Interfaces.DbReads;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.GeneralBooks;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.GeneralBooks;


public class GetGeneralBookByIdHandler
    : IRequestHandler<GetGeneralBookByIdQuerry, Result<GeneralBookDetailsReadModel>>
{
    private readonly IGeneralBookQueryService _bookQuerry;
    private readonly IImageStorageService _imageStorage;

    public GetGeneralBookByIdHandler(
        IGeneralBookQueryService generalBookQueryService,
        IImageStorageService imageStorageService)
    {
        _bookQuerry = generalBookQueryService;
        _imageStorage = imageStorageService;
    }

    public async Task<Result<GeneralBookDetailsReadModel>> Handle(
        GetGeneralBookByIdQuerry request,
        CancellationToken cancellationToken)
    {
        var book = await _bookQuerry.GetBookDetailsAsync(request.BookId, 10, cancellationToken);
        if (book is null)
            return Result.Fail(DomainErrorFactory.NotFound("GeneralBook", request.BookId));

        var enrichedBook = book with
        {
            CoverPhotoUrl = _imageStorage.GetPublicUrl(book.CoverPhotoUrl)
        };

        return Result.Ok(enrichedBook);
    }
}