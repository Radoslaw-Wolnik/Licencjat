// Backend.API/Controllers/AuthController.cs
using Backend.Application.DTOs.Auth;
using Backend.Application.Interfaces;
using Backend.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    
    public AuthController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        // Convert DTO to parameters for your domain model
        await _userService.RegisterUserAsync(
            request.Email,
            request.UserName,
            request.Password,
            request.FirstName,
            request.LastName,
            request.BirthDate);
            
        return Ok();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        await _userService.LoginUserAsync(
            request.Email, 
            request.Password, 
            request.RememberMe);
        // should we here throw things if the login is not sucessfull? 
        //return Ok(new { Message = "Login successful" });
        return Ok();
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

