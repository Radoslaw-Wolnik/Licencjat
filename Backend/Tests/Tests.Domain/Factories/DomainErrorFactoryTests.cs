using Backend.Domain.Errors;
using FluentAssertions;
using Xunit;

namespace Tests.Domain.Factories;

public class DomainErrorFactoryTests
{
    [Fact]
    public void NotFound_CreatesCorrectError()
    {
        // Arrange
        const string entity = "Book";
        var key = Guid.NewGuid();
        
        // Act
        var error = DomainErrorFactory.NotFound(entity, key);
        
        // Assert
        error.Should().BeEquivalentTo(new
        {
            Code = "Book.NotFound",
            Message = $"Book with id '{key}' was not found",
            Type = ErrorType.NotFound
        });
        error.Metadata.Should().ContainKeys("Entity", "Key");
    }

    [Fact]
    public void AlreadyExists_CreatesCorrectError()
    {
        // Arrange
        const string entity = "User";
        const string detail = "Email already registered";
        
        // Act
        var error = DomainErrorFactory.AlreadyExists(entity, detail);
        
        // Assert
        error.Should().BeEquivalentTo(new
        {
            Code = "User.Exists",
            Message = "Email already registered",
            Type = ErrorType.Conflict
        });
    }

    [Fact]
    public void LimitReached_CreatesCorrectError()
    {
        // Arrange
        const string entity = "SocialMedia";
        const string detail = "Maximum 10 links allowed";
        
        // Act
        var error = DomainErrorFactory.LimitReached(entity, detail);
        
        // Assert
        error.Should().BeEquivalentTo(new
        {
            Code = "SocialMedia.LimitReached",
            Message = "Maximum 10 links allowed",
            Type = ErrorType.Validation
        });
    }

    [Theory]
    [InlineData("User", "Password", ErrorType.Validation)]
    [InlineData("Auth", "Token", ErrorType.Unauthorized)]
    public void Invalid_CreatesCorrectError(string entity, string context, ErrorType type)
    {
        // Act
        var error = DomainErrorFactory.Invalid(entity, context, type);
        
        // Assert
        error.Should().BeEquivalentTo(new
        {
            Code = $"{entity}.Invalid{context}",
            Message = $"Invalid {context.ToLower()} for {entity.ToLower()}",
            Type = type
        });
    }

    [Fact]
    public void BadRequest_CreatesCorrectError()
    {
        // Act
        var error = DomainErrorFactory.BadRequest("Invalid.Format", "Invalid JSON format");
        
        // Assert
        error.Should().BeEquivalentTo(new
        {
            Code = "Invalid.Format",
            Message = "Invalid JSON format",
            Type = ErrorType.BadRequest
        });
    }

    [Fact]
    public void Forbidden_CreatesCorrectError()
    {
        // Act
        var error = DomainErrorFactory.Forbidden("Age.Restriction", "User is underage");
        
        // Assert
        error.Should().BeEquivalentTo(new
        {
            Code = "Age.Restriction",
            Message = "User is underage",
            Type = ErrorType.Forbidden
        });
    }

    [Fact]
    public void StorageError_CreatesCorrectError()
    {
        // Act
        var error = DomainErrorFactory.StorageError("Database connection failed");
        
        // Assert
        error.Should().BeEquivalentTo(new
        {
            Code = "Storage.Error",
            Message = "Storage error: Database connection failed",
            Type = ErrorType.StorageError
        });
    }

    [Fact]
    public void ServiceUnavailable_CreatesCorrectError()
    {
        // Act
        var error = DomainErrorFactory.ServiceUnavailable("Maintenance in progress");
        
        // Assert
        error.Should().BeEquivalentTo(new
        {
            Code = "Service.Unavailable",
            Message = "Service unavailable: Maintenance in progress",
            Type = ErrorType.ServiceUnavailable
        });
    }
}