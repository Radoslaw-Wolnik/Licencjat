using System.Threading.Channels;
using MediatR;
using FluentResults;
using Backend.Application.Interfaces;
using Backend.Domain.Common;
using Backend.Domain.Events; // for ThumbnailRequest

namespace Backend.Application.Commands.Users.ProfilePictures;

public class ConfirmUserProfilePictureCommandHandler
    : IRequestHandler<ConfirmUserProfilePictureCommand, Result>
{
    private readonly IImageStorageService _imageStorage;
    private readonly Channel<ThumbnailRequest> _channel;

    public ConfirmUserProfilePictureCommandHandler(
        IImageStorageService imageStorage,
        Channel<ThumbnailRequest> channel)
    {
        _imageStorage = imageStorage;
        _channel = channel;
    }

    public async Task<Result> Handle(
        ConfirmUserProfilePictureCommand request,
        CancellationToken cancellationToken)
    {
        // we could fetch the image key based on the user id from user repo
        // check existence
        if (!await _imageStorage.ExistsAsync(
                request.ImageObjectKey,
                cancellationToken))
        {
            return Result.Fail("The image was not uploaded");
        }

        // enqueue to the *instance*’s writer
        await _channel.Writer.WriteAsync(
            new ThumbnailRequest(request.ImageObjectKey, ThumbnailType.ProfilePicture),
            cancellationToken);

        // return immediately not waiting for background worker
        return Result.Ok();
    }
}
