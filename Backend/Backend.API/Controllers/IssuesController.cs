using AutoMapper;
using Backend.API.DTOs.Swaps;
using Backend.Application.Commands.Swaps.Issues;
using Backend.Infrastructure.Extensions;
using Backend.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/swaps/{swapId:guid}/issues")]
public sealed class IssuesController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public IssuesController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }
    [HttpPost]
    public async Task<IActionResult> Add(
        Guid swapId,
        [FromBody] AddIssueRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<AddIssueCommand>(request) with 
        { 
            SwapId = swapId,
            UserId = userId
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: issueId => CreatedAtAction(
                nameof(Get), 
                new { swapId, issueId }, 
                new { IssueId = issueId }),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpDelete("{issueId:guid}")]
    public async Task<IActionResult> Remove(
        Guid swapId,
        Guid issueId,
        [FromBody] RemoveIssueRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<RemoveIssueCommand>(request) with 
        { 
            SwapId = swapId,
            IssueId = issueId,
            UserId = userId
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }
}