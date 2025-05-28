using AutoMapper;
using Backend.API.DTOs.Swaps;
using Backend.Application.Commands.Swaps.Core;
using Backend.Infrastructure.Extensions;
using Backend.API.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Backend.API.DTOs.Common;
using Backend.API.DTOs.Swaps.Responses;
using Backend.Application.Querries.Swaps;
using Backend.Domain.Enums;

namespace Backend.API.Controllers;

[ApiController]
[Authorize]
[Route("api/swaps")]
public sealed class SwapsController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public SwapsController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] CreateSwapRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<CreateSwapCommand>(request) with 
        { 
            UserRequestingId = userId 
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => CreatedAtAction(
                nameof(Get), 
                new { id = result.Value }, 
                new { SwapId = result.Value }),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPut("{id:guid}/accept")]
    public async Task<IActionResult> Accept(
        Guid id,
        [FromBody] AcceptSwapRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<AcceptSwapCommand>(request) with 
        { 
            SwapId = id,
            UserAcceptingId = userId
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPut("{id:guid}/deny")]
    public async Task<IActionResult> Deny(Guid id)
    {
        var userId = User.GetUserId();
        var command = new DenySwapCommand(id, userId);
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateSwapRequest request)
    {
        var userId = User.GetUserId();
        var command = _mapper.Map<UpdateSwapCommand>(request) with 
        { 
            SwapId = id,
            UserId = userId
        };
        
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: () => NoContent(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = User.GetUserId();
        var command = new DeleteSwapCommand(id) 
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