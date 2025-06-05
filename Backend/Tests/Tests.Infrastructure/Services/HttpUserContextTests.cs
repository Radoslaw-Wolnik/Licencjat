using Backend.Application.Interfaces;
using Backend.Infrastructure.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Moq;
using System.Security.Claims;
using Xunit;

namespace Tests.Infrastructure.Services
{
    public class HttpUserContextTests
    {
        [Fact]
        public void UserId_ReturnsValue_WhenAuthenticated()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var httpContext = new DefaultHttpContext
            {
                User = CreateClaimsPrincipal(userId)
            };
            
            var accessor = MockHttpContextAccessor(httpContext);
            var context = new HttpUserContext(accessor);
            
            // Act
            var result = context.UserId;
            
            // Assert
            result.Should().Be(userId);
        }

        [Fact]
        public void UserId_ReturnsNull_WhenNotAuthenticated()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var accessor = MockHttpContextAccessor(httpContext);
            var context = new HttpUserContext(accessor);
            
            // Act
            var result = context.UserId;
            
            //System.InvalidOperationException
            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void UserId_ThrowsInvalidOperationException_WhenNotAuthenticated()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            var accessor = MockHttpContextAccessor(httpContext);
            var context = new HttpUserContext(accessor);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => {
                var userId = context.GetRequiredUserId();
            });

            exception.Message.Should().Be("User ID claim not found");
        }

        [Fact]
        public void IsAuthenticated_ReturnsTrue_WhenUserAuthenticated()
        {
            // Arrange
            var authenticatedIdentity = new ClaimsIdentity(authenticationType: "TestScheme");
    
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(authenticatedIdentity)
            };
            var accessor = MockHttpContextAccessor(httpContext);
            var context = new HttpUserContext(accessor);
            
            // Act
            var result = context.IsAuthenticated;
            
            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasClaim_ReturnsTrue_WhenClaimExists()
        {
            // Arrange
            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim("custom_claim", "value")
                }))
            };
            var accessor = MockHttpContextAccessor(httpContext);
            var context = new HttpUserContext(accessor);
            
            // Act
            var result = context.HasClaim("custom_claim", "value");
            
            // Assert
            result.Should().BeTrue();
        }

        private static ClaimsPrincipal CreateClaimsPrincipal(Guid userId)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(ClaimTypes.Name, "testuser")
            }));
        }

        private static IHttpContextAccessor MockHttpContextAccessor(HttpContext context)
        {
            var mock = new Mock<IHttpContextAccessor>();
            mock.Setup(a => a.HttpContext).Returns(context);
            return mock.Object;
        }
    }
}