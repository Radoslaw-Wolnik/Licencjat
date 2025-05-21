using Backend.Application.Interfaces.Repositories;
using Backend.Application.Interfaces.DbReads;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;
using Backend.Domain.Common;
using Backend.Domain.Enums;

namespace Backend.Application.Commands.Users.ProfilePictures;
public class UpdateUserProfilePictureCommandHandler
    : IRequestHandler<UpdateUserProfilePictureCommand, Result<string>>
{
    private readonly IWriteUserRepository _userRepo;
    private readonly IUserReadService _userRead;
    private readonly IImageStorageService _imageStorage;

    public UpdateUserProfilePictureCommandHandler(
        IWriteUserRepository userRepository,
        IUserReadService userReadService,
        IImageStorageService storage)
    {
        _userRepo = userRepository;
        _userRead = userReadService;
        _imageStorage  = storage;
    }

    public async Task<Result<string>> Handle(
        UpdateUserProfilePictureCommand request,
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

        // ask the storage service for objectKey
        var objectKey = _imageStorage.GenerateObjectKey(
            StorageDestination.GeneralBooks,
            user.Id,
            request.ProfilePictureFileName);

        // build your Photo metadata with the objectKey
        var photo = new Photo(objectKey);

        // change the profile picture in user
        user.UpdateProfilePicture(photo);

        // save the user - update the scalars
        var saveResult = await _userRepo.UpdateProfileAsync(user, cancellationToken);
        if (saveResult.IsFailed)
            return Result.Fail(saveResult.Errors);
        
        // ask the storage service for presigned URL
        var uploadUrl = await _imageStorage.GenerateUploadUrlAsync(objectKey);

        return Result.Ok((
            uploadUrl));
    }
}
