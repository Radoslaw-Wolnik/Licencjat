using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.UserBooks;
using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.UserBooks;

public class GetBookmarkByIdHandler
    : IRequestHandler<GetBookmarkByIdQuery, Result<BookmarkReadModel>>
{
    private readonly IUserBookQueryService _userBookQuery;
    private readonly IImageStorageService _imageStorage;

    public GetBookmarkByIdHandler(
        IUserBookQueryService userBookQueryService,
        IImageStorageService imageStorageService)
    {
        _userBookQuery = userBookQueryService;
        _imageStorage = imageStorageService;
    }

    public async Task<Result<BookmarkReadModel>> Handle(
        GetBookmarkByIdQuery request,
        CancellationToken cancellationToken)
    {
        var bookmark = await _userBookQuery.GetBookmarkByIdAsync(
            request.BookmarkId, 
            cancellationToken);

        if (bookmark is null)
        {
            return Result.Fail(DomainErrorFactory.NotFound("Bookmark", request.BookmarkId));
        }

        // If you need to process image URLs (e.g., for annotations)
        // if (!string.IsNullOrEmpty(bookmark.ImageUrl))
        // {
        //     bookmark = bookmark with 
        //     { 
        //         ImageUrl = _imageStorage.GetPublicUrl(bookmark.ImageUrl) 
        //     };
        // }

        return Result.Ok(bookmark);
    }
}