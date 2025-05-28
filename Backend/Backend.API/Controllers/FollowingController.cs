using Backend.API.DTOs.Users;
using Backend.Application.Commands.Users.Following;
using Backend.Infrastructure.Extensions;
using Backend.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users/following")]
public sealed class FollowingController : ControllerBase
{
    private readonly ISender _sender;
    
    public FollowingController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> FollowUser(
        [FromBody] FollowUserRequest request)
    {
        var userId = User.GetUserId();
        var command = new AddFollowedUserCommand(userId, request.UserFollowedId);
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpDelete("{userFollowedId:guid}")]
    public async Task<IActionResult> UnfollowUser(Guid userFollowedId)
    {
        var userId = User.GetUserId();
        var command = new RemoveFollowedUserCommand(userId, userFollowedId);
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }
}