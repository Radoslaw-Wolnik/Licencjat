using Backend.Domain.Common;
using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;
using Tests.Domain.Helpers;

namespace Tests.Domain.Common;

public class IssueTests
{
    private readonly Guid _validId = Guid.NewGuid();
    private readonly Guid _validUserId = Guid.NewGuid();
    private readonly Guid _validSubSwapId = Guid.NewGuid();

    [Fact]
    public void Create_WithValidParameters_ReturnsIssue()
    {
        // Arrange
        const string description = "Valid description";

        // Act
        var result = Issue.Create(_validId, _validUserId, _validSubSwapId, description);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Description = description.Trim()
        });
    }

    [Fact]
    public void Create_WithEmptyDescription_ReturnsError()
    {
        // Act
        var result = Issue.Create(_validId, _validUserId, _validSubSwapId, "  ");

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("Issue", "Description empty");
    }

    [Fact]
    public void Create_WithLongDescription_ReturnsError()
    {
        // Arrange
        var longDescription = new string('a', 1001);

        // Act
        var result = Issue.Create(_validId, _validUserId, _validSubSwapId, longDescription);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle();
        result.Errors[0].ShouldBeValidationError("Issue", "above 1000 characters");
    }

    [Fact]
    public void Create_TrimsDescription()
    {
        // Arrange
        const string description = "  Description with spaces  ";

        // Act
        var result = Issue.Create(_validId, _validUserId, _validSubSwapId, description);

        // Assert
        result.Value.Description.Should().Be("Description with spaces");
    }
}