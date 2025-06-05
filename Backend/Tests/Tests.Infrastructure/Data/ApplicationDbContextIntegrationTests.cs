using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Infrastructure.Data;
using Backend.Infrastructure.Entities;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Tests.Infrastructure;
using Xunit;

namespace Tests.Infrastructure.Data;

// important !!!
// please run docker app for performing this tests


public class ApplicationDbContextIntegrationTests : TestContainersBase, IAsyncLifetime
{
    private ApplicationDbContext _context = null!;

    protected override Task OnTestInitializedAsync()
    {
        _context = CreateDbContext();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task SaveChanges_SetsCreatedAtShadowProperty()
    {
        // Arrange
        var user = new UserEntity
        {
            UserName = "test",
            Email = "test@example.com",
            FirstName = "Barry", LastName = "Alan", City = "London", Country = "GB"
        };

        // Act
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Assert
        var createdAt = await _context.Users
            .Where(u => u.Id == user.Id)
            .Select(u => EF.Property<DateTime>(u, "CreatedAtTime"))
            .FirstAsync();

        createdAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task Reputation_CheckConstraint_RejectsInvalidValue()
    {
        // Arrange
        var user = new UserEntity
        {
            UserName = "test",
            Email = "test@example.com",
            Reputation = (float)6.0 // Invalid value
        };

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () =>
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        });
    }

    public new async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await base.DisposeAsync();
    }
}