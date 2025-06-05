using AutoMapper;
using Backend.Domain.Common;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Mapping;
using FluentAssertions;

namespace Tests.Infrastructure.Mapping;

public class IssueProfileTests
{
    private readonly IMapper _mapper;

    public IssueProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<IssueProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void IssueEntity_To_Issue_MapsCorrectly()
    {
        // Arrange
        var entity = new IssueEntity
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            SubSwapId = Guid.NewGuid(),
            Description = "Water damage to pages 50-75"
        };

        // Act
        var issue = _mapper.Map<Issue>(entity);

        // Assert
        issue.Id.Should().Be(entity.Id);
        issue.UserId.Should().Be(entity.UserId);
        issue.SubSwapId.Should().Be(entity.SubSwapId);
        issue.Description.Should().Be(entity.Description);
    }

    [Fact]
    public void Issue_To_IssueEntity_MapsCorrectly()
    {
        // Arrange
        var issue = new Issue(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            SubSwapId: Guid.NewGuid(),
            Description: "Broken spine"
        );

        // Act
        var entity = _mapper.Map<IssueEntity>(issue);

        // Assert
        entity.Id.Should().Be(issue.Id);
        entity.UserId.Should().Be(issue.UserId);
        entity.SubSwapId.Should().Be(issue.SubSwapId);
        entity.Description.Should().Be(issue.Description);
    }

    [Fact]
    public void IssueEntity_To_Issue_ThrowsForEmptyDescription()
    {
        // Arrange
        var entity = new IssueEntity
        {
            Description = "" // Invalid
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<Issue>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*Description empty*");
    }

    [Fact]
    public void IssueEntity_To_Issue_ThrowsForLongDescription()
    {
        // Arrange
        var longDesc = new string('x', 1001);
        var entity = new IssueEntity
        {
            Description = longDesc
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<Issue>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*Description too long*");
    }
}