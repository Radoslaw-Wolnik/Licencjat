using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.UserBooks;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.UserBooks;


public class GetUserBookByIdHandler
    : IRequestHandler<GetUserBookByIdQuerry, Result<UserBookDetailsReadModel?>>
{
    private readonly IUserBookQueryService _userBookQuery;
    private readonly IImageStorageService  _imageStorage;

    public GetUserBookByIdHandler(
        IUserBookQueryService userQueryService,
        IImageStorageService imageStorageService)
    {
        _userBookQuery = userQueryService;
        _imageStorage = imageStorageService;
    }

    public async Task<Result<UserBookDetailsReadModel?>> Handle(
        GetUserBookByIdQuerry request,
        CancellationToken cancellationToken)
    {
        var query = await _userBookQuery.GetBookDetailsAsync(request.UserBookId, cancellationToken);

        return Result.Ok(query);
        
        // mby change the profileurls for the official ones

    }
}