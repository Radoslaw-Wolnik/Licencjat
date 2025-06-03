using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using Backend.Domain.Factories;
using FluentAssertions;
using FluentResults;
using Xunit;

namespace Tests.Domain.Factories;

public class TimelineUpdateFactoryTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _swapId = Guid.NewGuid();

    [Fact]
    public void CreateRequested_ValidParameters_CreatesRequestedStatus()
    {
        // Act
        var result = TimelineUpdateFactory.CreateRequested(_userId, _swapId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            UserId = _userId,
            SwapId = _swapId,
            Status = TimelineStatus.Requested,
            Description = "Swap requested by user."
        });
    }

    [Theory]
    [InlineData(true, TimelineStatus.Accepted, "Swap request accepted.")]
    [InlineData(false, TimelineStatus.Declined, "Swap request declined.")]
    public void CreateResponse_ValidParameters_CreatesCorrectStatus(
        bool accepted, TimelineStatus expectedStatus, string expectedDescription)
    {
        // Act
        var result = TimelineUpdateFactory.CreateResponse(_userId, _swapId, accepted);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Status = expectedStatus,
            Description = expectedDescription
        });
    }

    [Fact]
    public void CreateMeetingUp_ValidParameters_CreatesMeetingUpStatus()
    {
        // Act
        var result = TimelineUpdateFactory.CreateMeetingUp(_userId, _swapId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Status = TimelineStatus.MeetingUp,
            Description = "Users agreed to meet in person."
        });
    }

    [Theory]
    [InlineData(50)]
    [InlineData(100)]
    public void CreateReadingProgress_ValidPage_CreatesReadingProgress(int currentPage)
    {
        // Act
        var result = TimelineUpdateFactory.CreateReadingProgress(_userId, _swapId, currentPage);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Status = TimelineStatus.ReadingBooks,
            Description = $"Reading progress: current page {currentPage}."
        });
    }

    [Fact]
    public void CreateWaitingForFinish_ValidParameters_CreatesCorrectStatus()
    {
        // Act
        var result = TimelineUpdateFactory.CreateWaitingForFinish(_userId, _swapId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Status = TimelineStatus.WaitingForFinish,
            Description = "One of you finished reading their book"
        });
    }

    [Fact]
    public void CreateFinishedReading_ValidParameters_CreatesFinishedStatus()
    {
        // Act
        var result = TimelineUpdateFactory.CreateFinishedReading(_userId, _swapId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Status = TimelineStatus.FinishedBooks,
            Description = "Finished reading books."
        });
    }

    [Fact]
    public void CreateCompleted_ValidParameters_CreatesFinishedStatus()
    {
        // Act
        var result = TimelineUpdateFactory.CreateCompleted(_userId, _swapId);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Status = TimelineStatus.Finished,
            Description = "Swap completed successfully. User gave their feedback"
        });
    }

    [Theory]
    [InlineData("Missing pages")]
    [InlineData("Book damaged")]
    public void CreateDispute_ValidParameters_CreatesDisputedStatus(string issueDetails)
    {
        // Act
        var result = TimelineUpdateFactory.CreateDispute(_swapId, _userId, issueDetails);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Status = TimelineStatus.Disputed,
            Description = $"Dispute raised: {issueDetails}"
        });
    }

    [Theory]
    [InlineData("Compensation provided")]
    [InlineData("Issue resolved")]
    public void CreateResolved_ValidParameters_CreatesResolvedStatus(string resolutionDetails)
    {
        // Act
        var result = TimelineUpdateFactory.CreateResolved(_swapId, _userId, resolutionDetails);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(new
        {
            Status = TimelineStatus.Resolved,
            Description = $"Dispute resolved: {resolutionDetails}"
        });
    }

    [Fact]
    public void Create_WithLongDescription_ReturnsError()
    {
        // Arrange
        var longDescription = new string('a', 101);
        
        // Act
        var result = TimelineUpdateFactory.CreateDispute(
            _swapId, _userId, longDescription);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => 
            e.Message.Contains("Description too long"));
    }

    [Fact]
    public void Create_WithEmptyUserId_ReturnsError()
    {
        // Act
        var result = TimelineUpdateFactory.CreateRequested(Guid.Empty, _swapId);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => 
            e.Message.Contains("User"));
    }
}