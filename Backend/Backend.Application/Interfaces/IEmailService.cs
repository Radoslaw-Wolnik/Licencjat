// Backend.Application/Interfaces/IEmailService.cs

namespace Backend.Application.Interfaces;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(string email, string resetLink);
    Task Send2faCodeAsync(string email, string code);
}