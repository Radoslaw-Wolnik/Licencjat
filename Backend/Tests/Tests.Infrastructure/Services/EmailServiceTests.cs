using Backend.Infrastructure.Services;
using Xunit;

namespace Tests.Infrastructure.Services;

public class EmailServiceTests
{
    [Fact (Skip = "NotImplemented Exception")]
    public async Task Send2faCodeAsync_DoesNotThrow()
    {
        // Arrange
        var service = new EmailService();
        
        // Act & Assert (no exception)
        await service.Send2faCodeAsync("test@example.com", "123456");
    }

    [Fact]
    public async Task SendPasswordResetEmailAsync_DoesNotThrow()
    {
        // Arrange
        var service = new EmailService();
        
        // Act & Assert (no exception)
        await service.SendPasswordResetEmailAsync("user@domain.com", "https://reset.link");
    }
}