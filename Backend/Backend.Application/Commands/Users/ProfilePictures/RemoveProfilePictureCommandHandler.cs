using Backend.Application.Interfaces.Repositories;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;

namespace Backend.Application.Commands.Users.ProfilePictures;
public class RemoveUserProfilePictureCommandHandler
    : IRequestHandler<RemoveUserProfilePictureCommand, Result>
{
    private readonly IWriteUserRepository _userRepo;
    private readonly IUserReadService _userRead;
    private readonly IImageStorageService _imageStorage;

    public RemoveUserProfilePictureCommandHandler(
        IWriteUserRepository userRepository,
        IUserReadService userReadService,
        IImageStorageService storage)
    {
        _userRepo = userRepository;
        _userRead = userReadService;
        _imageStorage  = storage;
    }

    public async Task<Result> Handle(
        RemoveUserProfilePictureCommand request,
        CancellationToken cancellationToken)
    {
        // get user
        var user = await _userRead.GetByIdAsync(request.UserId, cancellationToken);
        
        if (user == null)
            return Result.Fail(DomainErrorFactory.NotFound("User", request.UserId));

        // remove previous photo
        // - this could be done after succesfully uploading the profile picture 
        // but i would need to write another handler ... 
        var oldObjectKey = user.ProfilePicture?.Link;
        if (oldObjectKey != null)
            await _imageStorage.DeleteAsync(oldObjectKey, cancellationToken);

        // change the profile picture in user to null
        user.UpdateProfilePicture(null);

        // save the user - update the scalars
        var saveResult = await _userRepo.UpdateProfileAsync(user, cancellationToken);
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors);

        return Result.Ok();
    }
}
