using Backend.Application.Interfaces;
using Backend.Application.Interfaces.DbReads;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.GeneralBooks;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.GeneralBooks;


public class ListGeneralBooksHandler
    : IRequestHandler<ListGeneralBooksQuerry, Result<PaginatedResult<GeneralBookListItem>>>
{
    private readonly IGeneralBookQueryService _bookQuery;
    private readonly IImageStorageService  _imageStorage;

    public ListGeneralBooksHandler(
        IGeneralBookQueryService generalBookQueryService,
        IImageStorageService imageStorageService)
    {
        _bookQuery = generalBookQueryService;
        _imageStorage = imageStorageService;
    }

    public async Task<Result<PaginatedResult<GeneralBookListItem>>> Handle(
        ListGeneralBooksQuerry request,
        CancellationToken cancellationToken)
    {
        var pageOfBookItems = await _bookQuery.ListAsync(
            request.Title, request.Author, request.BookGenre,
            request.SortBy, request.Descending,
            request.Offset, request.Limit, cancellationToken);
        
        var updatedItems = pageOfBookItems.Items
            .Select(book => book with
            {
                CoverUrl = _imageStorage.GetPublicUrl(book.CoverUrl) // CoverUrl stores the key when fetched from the db for first time
            })
            .ToList();

        var updatedResult = new PaginatedResult<GeneralBookListItem>(updatedItems, pageOfBookItems.TotalCount);

        

        return Result.Ok(updatedResult);

    }
}