using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.UserBooks;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.UserBooks;


public class GetUserOwnBookProfileByIdHandler
    : IRequestHandler<GetUserOwnBookProfileByIdQuerry, Result<UserOwnBookProfileReadModel?>>
{
    private readonly IUserBookQueryService _userBookQuery;
    private readonly IImageStorageService  _imageStorage;

    public GetUserOwnBookProfileByIdHandler(
        IUserBookQueryService userQueryService,
        IImageStorageService imageStorageService)
    {
        _userBookQuery = userQueryService;
        _imageStorage = imageStorageService;
    }

    public async Task<Result<UserOwnBookProfileReadModel?>> Handle(
        GetUserOwnBookProfileByIdQuerry request,
        CancellationToken cancellationToken)
    {
        // check if user is owner of that userbook
        // if (profile.UserId != request.UserId)
        //     return Result.Fail(new ForbiddenError("Not owner"));

        var query = await _userBookQuery.GetOwnBookDetailsAsync(request.UserBookId, 10, cancellationToken);

        return Result.Ok(query);
        
        // mby change the profileurls for the official ones

    }
}