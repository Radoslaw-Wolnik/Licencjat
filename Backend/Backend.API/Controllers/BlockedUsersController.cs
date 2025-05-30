using Backend.API.DTOs.Users;
using Backend.Application.Commands.Users.Blocked;
using Backend.Infrastructure.Extensions;
using Backend.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Application.ReadModels.Common;
using Backend.API.DTOs.Common;
using Backend.Application.Querries.Users.Collections;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace Backend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users/blocked")]
public sealed class BlockedUsersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public BlockedUsersController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetBlockedUsers(
        [FromQuery] string? username,
        [FromQuery] bool descending = false,
        [FromQuery] int offset = 0,
        [FromQuery][Range(1, 100)] int limit = 20)
    {
        var userId = User.GetUserId();
        var query = new ListBlockedQuery(
            userId,
            username,
            descending,
            offset,
            limit);

        var result = await _sender.Send(query);

        return result.Match(
            paginated => Ok(_mapper.Map<PaginatedResponse<UserSmallResponse>>(paginated)),
            errors => errors.ToProblemDetailsResult()
        );
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