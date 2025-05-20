using Backend.Application.Interfaces.DbReads;
using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.SocialMedia;

public class UpdateSocialMediaCommandHandler
    : IRequestHandler<UpdateSocialMediaCommand, Result<SocialMediaLink>>
{
    private readonly IWriteUserRepository _userRepo;
    private readonly IUserReadService _userRead;
    public UpdateSocialMediaCommandHandler(
        IWriteUserRepository userRepo,
        IUserReadService userReadService
    )
    {
        _userRepo = userRepo;
        _userRead = userReadService;
    }

    public async Task<Result<SocialMediaLink>> Handle(
        UpdateSocialMediaCommand request,
        CancellationToken cancellationToken
    ) {
        var socialMedia = await _userRead.GetSocialMediaByIdAsync(request.SocialMediaId, cancellationToken);
        
        if (socialMedia == null)
            return Result.Fail(DomainErrorFactory.NotFound("SocialMediaLink", request.SocialMediaId));

        var newPlatform = request.Platform ?? socialMedia.Platform;
        var newUrl = request.Url ?? socialMedia.Url;

        var updatedResult = SocialMediaLink.Create(socialMedia.Id, newPlatform, newUrl);
        if (updatedResult.IsFailed)
            return Result.Fail(updatedResult.Errors);

        var persistance = await _userRepo.UpdateSingleSocialMediaAsync(updatedResult.Value, cancellationToken);
        if (persistance.IsFailed)
            return Result.Fail(persistance.Errors);
        
        return Result.Ok(updatedResult.Value);
    }
}
