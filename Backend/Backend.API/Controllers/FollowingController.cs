using Backend.API.DTOs.Users;
using Backend.Application.Commands.Users.Following;
using Backend.Infrastructure.Extensions;
using Backend.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.API.DTOs.Common;
using Backend.Application.Querries.Users.Collections;
using System.ComponentModel.DataAnnotations;
using AutoMapper;

namespace Backend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users/following")]
public sealed class FollowingController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public FollowingController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetFollowedUsers(
        [FromQuery] string? username,
        [FromQuery] bool descending = false,
        [FromQuery] int offset = 0,
        [FromQuery][Range(1, 100)] int limit = 20)
    {
        var userId = User.GetUserId();
        var query = new ListFollowedQuery(
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