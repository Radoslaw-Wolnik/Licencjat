using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Users.SocialMedia;

public class AddSocialMediaCommandHandler
    : IRequestHandler<AddSocialMediaCommand, Result<SocialMediaLink>>
{
    private readonly IWriteUserRepository _userRepo;
    public AddSocialMediaCommandHandler(
        IWriteUserRepository userRepo
    ) {
        _userRepo = userRepo;
    }

    public async Task<Result<SocialMediaLink>> Handle(
        AddSocialMediaCommand request,
        CancellationToken cancellationToken
    ) {
        var socialMediaLinkResult = SocialMediaLink.Create(Guid.NewGuid(), request.Platform, request.Url);
        if (socialMediaLinkResult.IsFailed)
            return Result.Fail(socialMediaLinkResult.Errors);

        var persistance = await _userRepo.AddSocialMediaAsync(request.UserId, socialMediaLinkResult.Value, cancellationToken);
        if (persistance.IsFailed)
            return Result.Fail(persistance.Errors);
        
        return Result.Ok(socialMediaLinkResult.Value);
    }
}
