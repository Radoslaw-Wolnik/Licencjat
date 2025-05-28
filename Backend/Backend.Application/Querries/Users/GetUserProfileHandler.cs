using Backend.Application.Interfaces;
using Backend.Application.Interfaces.Queries;
using Backend.Application.ReadModels.Users;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Users;


public class GetUserProfileHandler
    : IRequestHandler<GetUserProfileQuerry, Result<UserProfileReadModel?>>
{
    private readonly IUserQueryService _userQuery;
    private readonly IImageStorageService  _imageStorage;

    public GetUserProfileHandler(
        IUserQueryService userQueryService,
        IImageStorageService imageStorageService)
    {
        _userQuery = userQueryService;
        _imageStorage = imageStorageService;
    }

    public async Task<Result<UserProfileReadModel?>> Handle(
        GetUserProfileQuerry request,
        CancellationToken cancellationToken)
    {
        var query = await _userQuery.GetDetailsAsync(request.UserId, cancellationToken);

        return Result.Ok(query);
        
        // mby change the profileurls for the official ones

    }
}