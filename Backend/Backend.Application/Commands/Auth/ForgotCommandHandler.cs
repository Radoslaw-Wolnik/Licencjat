/*
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;

namespace Backend.Application.Commands.Auth;

public async Task<Result<bool>> Handle(
    ForgotCommand command, 
    CancellationToken ct)
{
    var user = await _userManager.FindByEmailAsync(command.Email);
    if (user == null) 
        return true; // Don't reveal non-existent emails
    
    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
    var resetLink = $"{_config["ClientUrl"]}/reset-password?token={WebUtility.UrlEncode(token)}&email={command.Email}";
    
    await _emailService.SendPasswordResetEmailAsync(user.Email, resetLink);
    
    return true;
}
*/