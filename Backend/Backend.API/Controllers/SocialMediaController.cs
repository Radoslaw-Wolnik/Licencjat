using AutoMapper;
using Backend.API.DTOs.Users;
using Backend.Application.Commands.Users.SocialMedia;
using Backend.Infrastructure.Extensions;
using Backend.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.API.DTOs.Users.Responses;
using Backend.Application.Querries.Users.Collections;

namespace Backend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/users/social-media")]
public sealed class SocialMediaController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public SocialMediaController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSocialMedia()
    {
        var userId = User.GetUserId();
        var query = new ListSocialMediaQuery(userId);
        
        var result = await _sender.Send(query);

        return result.Match(
            onSuccess: socialMedia => Ok(
                socialMedia.Select(s => _mapper.Map<SocialMediaResponse>(s))
            ),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpGet("{socialMediaId:guid}")]
    public async Task<IActionResult> GetSocialMediaById(Guid socialMediaId)
    {
        var query = new GetSocialMediaByIdQuerry(socialMediaId);
        
        var result = await _sender.Send(query);

        return result.Match(
            onSuccess: socialMedia => Ok(_mapper.Map<SocialMediaResponse>(socialMedia)),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPost]
    public async Task<IActionResult> AddSocialMedia(
        [FromBody] AddSocialMediaRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<AddSocialMediaCommand>(request) with 
        { 
            UserId = userId 
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: socialMedia => CreatedAtAction(
                nameof(GetSocialMediaById), 
                new { id = socialMedia.Id }, 
                _mapper.Map<SocialMediaResponse>(socialMedia)),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPut("{socialMediaId:guid}")]
    public async Task<IActionResult> UpdateSocialMedia(
        Guid socialMediaId,
        [FromBody] UpdateSocialMediaRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<UpdateSocialMediaCommand>(request) with
        {
            SocialMediaId = socialMediaId,
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: socialMedia => Ok(_mapper.Map<SocialMediaResponse>(socialMedia)),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpDelete("{socialMediaId:guid}")]
    public async Task<IActionResult> RemoveSocialMedia(Guid socialMediaId)
    {
        var userId = User.GetUserId();
        var command = new RemoveCommand(socialMediaId);
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }
}