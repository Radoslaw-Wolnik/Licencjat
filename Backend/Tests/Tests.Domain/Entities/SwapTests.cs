using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentAssertions;

namespace Tests.Domain.Entities;

public class SwapTests
{
    private readonly Guid _requestingUserId = Guid.NewGuid();
    private readonly Guid _acceptingUserId = Guid.NewGuid();
    private readonly UserBook _requestingBook;
    private readonly Swap _swap;
    private LanguageCode Language = LanguageCode.Create("en").ValueOrDefault;

    public SwapTests()
    {
        _requestingBook = UserBook.Create(
            Guid.NewGuid(), _requestingUserId, Guid.NewGuid(),
            BookStatus.Finished, BookState.Available, Language,
            300, new Photo("cover.jpg")).Value;

        _swap = Swap.Create(_requestingUserId, _requestingBook, _acceptingUserId, DateOnly.FromDateTime(DateTime.UtcNow)).Value;
    }

    [Fact]
    public void Create_WithValidParameters_CreatesSwap()
    {
        // Assert
        _swap.Should().BeEquivalentTo(new
        {
            Status = SwapStatus.Requested,
            CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
        });
        _swap.SubSwapRequesting.UserId.Should().Be(_requestingUserId);
        _swap.SubSwapAccepting.UserId.Should().Be(_acceptingUserId);
    }

    [Fact]
    public void InitialBookReading_ByAcceptingUser_SetsBook()
    {
        // Arrange
        var acceptingBook = UserBook.Create(
            Guid.NewGuid(), _acceptingUserId, Guid.NewGuid(),
            BookStatus.Finished, BookState.Available, Language, 
            300, new Photo("cover.jpg")).Value;
        
        // Act
        var result = _swap.InitialBookReading(_acceptingUserId, acceptingBook);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        _swap.SubSwapAccepting.UserBookReading.Should().BeEquivalentTo(acceptingBook);
    }

    [Fact]
    public void InitialBookReading_ByWrongUser_ReturnsError()
    {
        // Arrange
        var book = UserBook.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            BookStatus.Finished, BookState.Available, Language, 
            300, new Photo("cover.jpg")).Value;
        
        // Act
        var result = _swap.InitialBookReading(_requestingUserId, book);
        
        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void UpdatePageReading_ValidUserAndPage_UpdatesPage()
    {
        // Act
        var result = _swap.UpdatePageReading(_requestingUserId, 50);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        _swap.SubSwapRequesting.PageAt.Should().Be(50);
    }

    [Fact]
    public void AddIssue_ValidUser_AddsIssue()
    {
        // Arrange
        var issue = new Issue(Guid.NewGuid(), _requestingUserId, _swap.SubSwapRequesting.Id, "Test issue");
        
        // Act
        var result = _swap.AddIssue(_requestingUserId, issue);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        _swap.SubSwapRequesting.Issue.Should().BeEquivalentTo(issue);
    }

    [Fact]
    public void UpdateStatus_ChangesStatus()
    {
        // Act
        _swap.UpdateStaus(SwapStatus.Completed);
        
        // Assert
        _swap.Status.Should().Be(SwapStatus.Completed);
    }
}