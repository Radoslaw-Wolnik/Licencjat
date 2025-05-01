using Backend.Application.Interfaces;

namespace Backend.Infrastructure.Services;

public class EmailService : IEmailService
{
    public Task Send2faCodeAsync(string email, string code)
    {
        throw new NotImplementedException();
    }

    public Task SendPasswordResetEmailAsync(string email, string resetLink)
    {
        // Integrate with your email provider (SendGrid, SMTP, etc.)
        return Task.CompletedTask;
    }
}