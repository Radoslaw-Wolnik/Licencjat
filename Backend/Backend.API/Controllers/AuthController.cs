using AutoMapper;
using Backend.API.DTOs.Auth;
using Backend.API.Extensions;
using Backend.Application.Commands.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly ISender _sender;
    private readonly IMapper _mapper;
    
    public AuthController(ISender sender, IMapper mapper)
    {
        _sender = sender;
        _mapper = mapper;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        var command = _mapper.Map<RegisterCommand>(request);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: userId => CreatedAtAction(
                nameof(Register), 
                new { userId }, 
                new { UserId = userId }),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        var command = _mapper.Map<LoginCommand>(request);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: user => Ok(_mapper.Map<LoginResponse>(new {
                UserId = user.Id,
                Username = user.Username
            })),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] ForgotPasswordRequest request)
    {
        var command = _mapper.Map<ForgotCommand>(request);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: _ => Ok(),
            onFailure: errors => errors.ToProblemDetailsResult()
        );
    }
}