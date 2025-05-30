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
[Route("api/users/followers")]
public sealed class FollowersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;

    public FollowersController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetFollowers(
        [FromQuery] string? username,
        [FromQuery] bool descending = false,
        [FromQuery] int offset = 0,
        [FromQuery][Range(1, 100)] int limit = 20)
    {
        var userId = User.GetUserId();
        var query = new ListFollowersQuery(
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
}