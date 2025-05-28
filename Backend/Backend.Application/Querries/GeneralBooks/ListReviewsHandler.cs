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


public class ListReviewsHandler
    : IRequestHandler<ListReviewsQuerry, Result<PaginatedResult<ReviewReadModel>>>
{
    private readonly IGeneralBookQueryService _bookQuery;
    private readonly IImageStorageService  _imageStorage;

    public ListReviewsHandler(
        IGeneralBookQueryService generalBookQueryService,
        IImageStorageService imageStorageService)
    {
        _bookQuery = generalBookQueryService;
        _imageStorage = imageStorageService;
    }

    public async Task<Result<PaginatedResult<ReviewReadModel>>> Handle(
        ListReviewsQuerry request,
        CancellationToken cancellationToken)
    {
        var pageOfReviewsItems = await _bookQuery.GetPaginatedReviewsAsync(
            request.GeneralBookId,
            request.SortBy, request.Descending,
            request.Offset, request.Limit, cancellationToken);
        
        // mby change the profileurls for the official ones

        return Result.Ok(pageOfReviewsItems);

    }
}