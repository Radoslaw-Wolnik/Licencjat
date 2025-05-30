using AutoMapper;
using Backend.API.DTOs.Swaps;
using Backend.Application.Commands.Swaps.Feedbacks;
using Backend.Infrastructure.Extensions;
using Backend.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.Application.Querries.Swaps;
using Backend.API.DTOs.Swaps.Responses;

namespace Backend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/swaps/{swapId:guid}/feedbacks")]
public sealed class FeedbacksController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public FeedbacksController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }

    [HttpGet("{feedbackId:guid}")]
    public async Task<IActionResult> Get(Guid swapId, Guid feedbackId)
    {
        var query = new GetFeedbackByIdQuery(feedbackId);
        var result = await _sender.Send(query);
        
        return result.Match(
            feedback => Ok(_mapper.Map<FeedbackResponse>(feedback)),
            errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPost]
    public async Task<IActionResult> Add(
        Guid swapId,
        [FromBody] AddFeedbackRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<AddFeedbackCommand>(request) with 
        { 
            SwapId = swapId,
            UserId = userId
        };
        
        var result = await _sender.Send(command);

        return result.Match<Guid>(
            onSuccess: feedbackId => CreatedAtAction(
                nameof(Get), 
                new { swapId, feedbackId }, 
                new { FeedbackId = feedbackId }),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }
}