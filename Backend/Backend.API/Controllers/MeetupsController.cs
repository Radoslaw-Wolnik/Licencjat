using AutoMapper;
using Backend.API.DTOs.Swaps;
using Backend.Application.Commands.Swaps.Meetups;
using Backend.Infrastructure.Extensions;
using Backend.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.API.DTOs.Swaps.Responses;

[ApiController]
[Authorize]
[Route("api/swaps/{swapId:guid}/meetups")]
public sealed class MeetupsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public MeetupsController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }
    [HttpPost]
    public async Task<IActionResult> Add(
        Guid swapId,
        [FromBody] AddMeetupRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<AddMeetupCommand>(request) with 
        { 
            SwapId = swapId,
            UserId = userId
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: meetupId => CreatedAtAction(
                nameof(Get), 
                new { swapId, meetupId }, 
                new { MeetupId = meetupId }),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPut("{meetupId:guid}")]
    public async Task<IActionResult> Update(
        Guid swapId,
        Guid meetupId,
        [FromBody] UpdateMeetupRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<UpdateMeetupCommand>(request) with 
        { 
            MeetupId = meetupId,
            UserId = userId
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: meetup => Ok(_mapper.Map<MeetupResponse>(meetup)),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpDelete("{meetupId:guid}")]
    public async Task<IActionResult> Remove(
        Guid swapId,
        Guid meetupId)
    {
        var userId = User.GetUserId();
        var command = new RemoveMeetupCommand(meetupId) 
        { 
            Metadata = { ["UserId"] = userId } 
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }
}