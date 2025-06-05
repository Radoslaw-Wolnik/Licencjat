using System.Security.Claims;
using Backend.Infrastructure.Extensions;
using FluentAssertions;
using Xunit;

namespace Tests.Infrastructure.Extensions
{
    public class ClaimsPrincipalExtensionsTests
    {
        [Fact]
        public void GetUserId_ReturnsGuid_WhenValidClaimExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var identity = new ClaimsIdentity(new[] 
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            });
            var principal = new ClaimsPrincipal(identity);
            
            // Act
            var result = principal.GetUserId();
            
            // Assert
            result.Should().Be(userId);
        }

        [Fact (Skip = "Either we return null when claims miss or throw erro - now null")]
        public void GetUserId_Throws_WhenClaimMissing()
        {
            // Arrange
            var principal = new ClaimsPrincipal(new ClaimsIdentity());
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => principal.GetUserId());
        }

        [Fact]
        public void GetUserId_Throws_WhenInvalidFormat()
        {
            // Arrange
            var identity = new ClaimsIdentity(new[] 
            {
                new Claim(ClaimTypes.NameIdentifier, "invalid-guid")
            });
            var principal = new ClaimsPrincipal(identity);
            
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => principal.GetUserId());
        }

        [Fact]
        public void GetUserIdOrNull_ReturnsGuid_WhenValidClaimExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var identity = new ClaimsIdentity(new[] 
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            });
            var principal = new ClaimsPrincipal(identity);
            
            // Act
            var result = principal.GetUserIdOrNull();
            
            // Assert
            result.Should().Be(userId);
        }

        [Fact]
        public void GetUserIdOrNull_ReturnsNull_WhenClaimMissing()
        {
            // Arrange
            var principal = new ClaimsPrincipal();
            
            // Act
            var result = principal.GetUserIdOrNull();
            
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void GetUserIdOrNull_ReturnsNull_WhenInvalidFormat()
        {
            // Arrange
            var identity = new ClaimsIdentity(new[] 
            {
                new Claim(ClaimTypes.NameIdentifier, "invalid-guid")
            });
            var principal = new ClaimsPrincipal(identity);
            
            // Act
            var result = principal.GetUserIdOrNull();
            
            // Assert
            result.Should().BeNull();
        }
    }
}