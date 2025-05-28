using Backend.API.DTOs.Users;
using Backend.Application.Commands.Users.Blocked;
using Backend.Infrastructure.Extensions;
using Backend.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users/blocked")]
public sealed class BlockedUsersController : ControllerBase
{
    private readonly ISender _sender;
    
    public BlockedUsersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost]
    public async Task<IActionResult> BlockUser(
        [FromBody] BlockUserRequest request)
    {
        var userId = User.GetUserId();
        var command = new AddBlockedUserCommand(userId, request.UserBlockedId);
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpDelete("{userBlockedId:guid}")]
    public async Task<IActionResult> UnblockUser(Guid userBlockedId)
    {
        var userId = User.GetUserId();
        var command = new RemoveBlockedUserCommand(userId, userBlockedId);
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }
}