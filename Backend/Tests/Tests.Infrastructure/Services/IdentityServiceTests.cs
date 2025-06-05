using Backend.Domain.Errors;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Services;
using FluentAssertions;
using FluentResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Tests.Infrastructure.Services;

public class IdentityServiceTests
{
    private readonly Mock<UserManager<UserEntity>> _userManagerMock;
    private readonly IdentityService _service;


    public IdentityServiceTests()
    {
        var storeMock = new Mock<IUserStore<UserEntity>>().Object;

        var identityOptions = new IdentityOptions();
        var options = Options.Create(identityOptions);

        var passwordHasherMock = new Mock<IPasswordHasher<UserEntity>>();
        var passwordHasher = passwordHasherMock.Object;

        var userValidators = new List<IUserValidator<UserEntity>>();
        var passwordValidators = new List<IPasswordValidator<UserEntity>>();

        var lookupNormalizerMock = new Mock<ILookupNormalizer>();
        var lookupNormalizer = lookupNormalizerMock.Object;

        var errorDescriber = new IdentityErrorDescriber();

        var services = new Mock<IServiceProvider>().Object;

        var loggerMock = new Mock<ILogger<UserManager<UserEntity>>>();
        var logger = loggerMock.Object;

        _userManagerMock = new Mock<UserManager<UserEntity>>(
            storeMock,
            options,
            passwordHasher,
            userValidators,
            passwordValidators,
            lookupNormalizer,
            errorDescriber,
            services,
            logger
        )
        {
            // (Optional) If you want to have the mock call the real methods by default, uncomment this:
            // CallBase = true
        };

        _service = new IdentityService(_userManagerMock.Object);
    }

    [Fact]
    public async Task CreateUserWithPasswordAsync_Success_ReturnsUserId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserEntity { 
            Id = userId,
            FirstName = "Test",
            LastName = "User",
            City = "London",
            Country = "UK",
            BirthDate = new DateOnly(1990, 1, 1)
        };
        
        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .Callback<UserEntity, string>((u, _) => u.Id = userId)
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _service.CreateUserWithPasswordAsync(
            userId, "test@example.com", "user", "Password123!", "John", "Doe", "London", "UK", new DateOnly(1990, 1, 1));

        // Assert
        result.Value.Should().Be(userId);
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CreateUserWithPasswordAsync_DuplicateEmail_ReturnsConflict()
    {
        // Arrange
        var errors = new[] { new IdentityError { Code = "DuplicateEmail", Description = "Email exists" } };
        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(errors));

        // Act
        var result = await _service.CreateUserWithPasswordAsync(
            Guid.NewGuid(), "duplicate@example.com", "user", "Password123!", "John", "Doe", "Paris", "FR", new DateOnly(2000, 5, 15));

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.First().Should().BeOfType<DomainError>();

        var domainError = (DomainError)result.Errors[0];

        domainError.Type.Should().Be(ErrorType.Conflict);
        domainError.Message.Should().Contain("Email exists");
    }

    [Fact]
    public async Task CreateUserWithPasswordAsync_WeakPassword_ReturnsValidationError()
    {
        // Arrange
        var errors = new[] { new IdentityError { Code = "PasswordTooShort", Description = "Minimum 6 chars" } };
        _userManagerMock.Setup(m => m.CreateAsync(It.IsAny<UserEntity>(), It.IsAny<string>()))
            .ReturnsAsync(IdentityResult.Failed(errors));

        // Act
        var result = await _service.CreateUserWithPasswordAsync(
            Guid.NewGuid(), "test@example.com", "user", "short", "Jane", "Smith", "Berlin", "DE", new DateOnly(1985, 3, 10));

        // Assert
        result.IsFailed.Should().BeTrue();

        result.Errors.First().Should().BeOfType<DomainError>();

        var domainError = (DomainError)result.Errors[0];

        domainError.Type.Should().Be(ErrorType.Validation);
        domainError.Message.Should().Contain("Invalid minimum 6 chars for");
    }
}