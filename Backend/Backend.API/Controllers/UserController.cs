using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Backend.API.DTOs.Common;
using Backend.API.DTOs.Users;
using Backend.API.DTOs.Users.Responses;
using Backend.API.Extensions;
using Backend.Application.Commands.Users.Core;
using Backend.Application.Commands.Users.ProfilePictures;
using Backend.Application.Querries.Users;
using Backend.Domain.Enums.SortBy;
using Backend.Infrastructure.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public UsersController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet("{userId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetUserProfile(Guid userId)
    {
        var query = new GetUserProfileQuerry(userId);
        var result = await _sender.Send(query);

        return result.Match(
            userProfile => userProfile is null 
                ? NotFound() 
                : Ok(_mapper.Map<UserProfileFullResponse>(userProfile)),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> SearchUsers(
        [FromQuery] string? userName = null,
        [FromQuery] float? reputation = null,
        [FromQuery] string? city = null,
        [FromQuery] string? country = null,
        [FromQuery] SortUsersBy sortBy = SortUsersBy.UserName,
        [FromQuery] bool descending = false,
        [FromQuery] int offset = 0,
        [FromQuery][Range(1, 100)] int limit = 20)
    {
        var query = new ListUsersQuerry(
            userName,
            reputation,
            city,
            country,
            sortBy,
            descending,
            offset,
            limit
        );

        var result = await _sender.Send(query);

        return result.Match(
            paginated => 
            {
                var response = new PaginatedResponse<UserSmallResponse>(
                    _mapper.Map<List<UserSmallResponse>>(paginated.Items),
                    paginated.TotalCount,
                    offset,
                    limit
                );
                return Ok(response);
            },
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile(
        [FromBody] UpdateProfileRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<UpdateUserProfileCommand>(request) with 
        { 
            UserId = userId 
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: user => Ok(_mapper.Map<UserProfileResponse>(user)),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpDelete("me")]
    public async Task<IActionResult> DeleteUser()
    {
        var userId = User.GetUserId();
        var command = new DeleteUserCommand(userId);
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPut("me/profile-picture")]
    public async Task<IActionResult> UpdateProfilePicture(
        [FromBody] UpdateProfilePictureRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<UpdateUserProfilePictureCommand>(request) with 
        { 
            UserId = userId 
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: newKey => Ok(new { ImageObjectKey = newKey }),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPost("me/profile-picture/confirm")]
    public async Task<IActionResult> ConfirmProfilePicture(
        [FromBody] ConfirmProfilePictureRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<ConfirmUserProfilePictureCommand>(request) with 
        { 
            UserId = userId 
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpDelete("me/profile-picture")]
    public async Task<IActionResult> RemoveProfilePicture()
    {
        var userId = User.GetUserId();
        var command = new RemoveUserProfilePictureCommand(userId);
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }
}