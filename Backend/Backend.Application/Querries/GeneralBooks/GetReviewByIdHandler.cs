using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.GeneralBooks;
using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.GeneralBooks;

public class GetReviewByIdHandler
    : IRequestHandler<GetReviewByIdQuery, Result<ReviewReadModel>>
{
    private readonly IGeneralBookQueryService _bookQuery;
    private readonly IImageStorageService _imageStorage;

    public GetReviewByIdHandler(
        IGeneralBookQueryService generalBookQueryService,
        IImageStorageService imageStorageService)
    {
        _bookQuery = generalBookQueryService;
        _imageStorage = imageStorageService;
    }

    public async Task<Result<ReviewReadModel>> Handle(
        GetReviewByIdQuery request,
        CancellationToken cancellationToken)
    {
        var review = await _bookQuery.GetReviewByIdAsync(
            request.ReviewId, 
            cancellationToken);

        if (review is null)
        {
            return Result.Fail(DomainErrorFactory.NotFound("Review", request.ReviewId));
        }

        // Update profile URL if exists
        // honestly there must be better way of doing this
        // mby some global middleware or sth like that
        if (!string.IsNullOrEmpty(review.User?.ProfilePictureUrl))
        {
            var UserPublicUrl = review.User with
            {
                ProfilePictureUrl = _imageStorage.GetPublicUrl(review.User.ProfilePictureUrl)
            };
            var fixedReview = review with
            {
                User = UserPublicUrl
            };
            return Result.Ok(fixedReview);
        }

        

        return Result.Ok(review);
    }
}