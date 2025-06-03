using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Domain.Errors;
using FluentAssertions;

namespace Tests.Domain.Entities;

public class SubSwapTests
{
    private readonly Guid _userId = Guid.NewGuid();
    private readonly UserBook _userBook;
    private readonly SubSwap _subSwap;
    private LanguageCode Language = LanguageCode.Create("en").ValueOrDefault;

    public SubSwapTests()
    {
        _userBook = UserBook.Create(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            BookStatus.Finished, BookState.Available, Language,
            300, new Photo("cover.jpg")).Value;

        _subSwap = SubSwap.Initial(_userId, _userBook);
    }

    [Fact]
    public void InitialBook_WhenNull_AddsBook()
    {
        // Arrange
        var subSwap = SubSwap.Initial(_userId, null);
        
        // Act
        var result = subSwap.InitialBook(_userBook);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        subSwap.UserBookReading.Should().BeEquivalentTo(_userBook);
    }

    [Fact]
    public void InitialBook_WhenBookAlreadySet_ReturnsError()
    {
        // Act
        var result = _subSwap.InitialBook(_userBook);
        
        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().Contain(e => e.Message.Contains("Cannot change"));
    }

    [Theory]
    [InlineData(10)]
    [InlineData(0)]
    public void UpdatePageAt_ValidPage_UpdatesPage(int newPage)
    {
        // Act
        var result = _subSwap.UpdatePageAt(newPage);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        _subSwap.PageAt.Should().Be(newPage);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(301)]
    public void UpdatePageAt_InvalidPage_ReturnsError(int newPage)
    {
        // Act
        var result = _subSwap.UpdatePageAt(newPage);
        
        // Assert
        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public void AddIssue_SetsIssue()
    {
        // Arrange
        var issue = new Issue(Guid.NewGuid(), _userId, _subSwap.Id, "Test issue");
        
        // Act
        var result = _subSwap.AddIssue(issue);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        _subSwap.Issue.Should().BeEquivalentTo(issue);
    }

    [Fact]
    public void UpdateIssue_WithValidIssue_UpdatesIssue()
    {
        // Arrange
        var issue = new Issue(Guid.NewGuid(), _userId, _subSwap.Id, "Initial issue");
        _subSwap.AddIssue(issue);
        var updatedIssue = new Issue(issue.UserId, _userId, issue.SubSwapId, "Updated issue");
        
        // Act
        var result = _subSwap.UpdateIssue(updatedIssue);
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        _subSwap.Issue.Should().BeEquivalentTo(updatedIssue);
    }
}