using AutoMapper;
using Backend.API.DTOs.Users;
using Backend.API.DTOs.Users.Responses;
using Backend.API.Extensions;
using Backend.Application.Commands.Users.Core;
using Backend.Application.Commands.Users.ProfilePictures;
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