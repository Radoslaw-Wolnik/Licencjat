// Backend.API/Controllers/AuthController.cs
using AutoMapper;
using Backend.API.Extensions;
using Backend.Application.DTOs.Commands.Auth;
using Backend.Application.Commands.Auth;
using Backend.Application.Interfaces;
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
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var command = _mapper.Map<RegisterCommand>(request);
        var result = await _sender.Send(command);

        return result.Match(
        onSuccess: userId => CreatedAtAction(
            nameof(Register), 
            new { userId }, 
            new { UserId = userId }),
        onFailure: errors =>
        {
        var problemDetails = errors.ToProblemDetails();
        return Problem(
            instance: "api/register",
            title: problemDetails.Title,
            statusCode: problemDetails.Status,
            type: problemDetails.Type,
            detail: problemDetails.Extensions.ToString()); // this is a dictionary i suppose
        });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var command = _mapper.Map<LoginCommand>(request);
        var result = await _sender.Send(command);

        return result.Match(
            onSuccess: user => Ok(_mapper.Map<LoginResponse>(user)),
            onFailure: errors => Problem(
                instance: "api/login",
                title: "Login error",
                statusCode: StatusCodes.Status401Unauthorized,
                detail: string.Join(", ", errors.Select(e => e.Message)))
            // onFailure: errors => errors.ToProblemDetailsResult().ToString()
        );
    }





    // rewrite this one to be better
    /*
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Ok(); // Don't reveal existence
        

        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetLink = $"https://yourapp.com/reset-password?token={WebUtility.UrlEncode(token)}&email={request.Email}";

        if (string.IsNullOrEmpty(user.Email))
            return BadRequest("User does not have a valid email address.");
        
        await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);
        
        return Ok();
    }
    */

}

