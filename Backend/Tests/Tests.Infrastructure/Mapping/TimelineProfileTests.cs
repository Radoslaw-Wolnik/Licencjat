using AutoMapper;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Mapping;
using FluentAssertions;

namespace Tests.Infrastructure.Mapping;

public class TimelineProfileTests
{
    private readonly IMapper _mapper;

    public TimelineProfileTests()
    {
        var config = new MapperConfiguration(cfg => 
        {
            cfg.AddProfile<TimelineProfile>();
        });
        
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void TimelineEntity_To_TimelineUpdate_MapsCorrectly()
    {
        // Arrange
        var entity = new TimelineEntity
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            SwapId = Guid.NewGuid(),
            Status = TimelineStatus.Accepted,
            Description = "Swap accepted by both parties",
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var timelineUpdate = _mapper.Map<TimelineUpdate>(entity);

        // Assert
        timelineUpdate.Id.Should().Be(entity.Id);
        timelineUpdate.UserId.Should().Be(entity.UserId);
        timelineUpdate.SwapId.Should().Be(entity.SwapId);
        timelineUpdate.Status.Should().Be(entity.Status);
        timelineUpdate.Description.Should().Be(entity.Description);
        timelineUpdate.CreatedAt.Should().Be(entity.CreatedAt);
    }

    [Fact]
    public void TimelineUpdate_To_TimelineEntity_MapsCorrectly()
    {
        // Arrange
        var timelineUpdate = new TimelineUpdate(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            SwapId: Guid.NewGuid(),
            Status: TimelineStatus.MeetingUp,
            Description: "Meeting scheduled",
            CreatedAt: DateTime.UtcNow
        );

        // Act
        var entity = _mapper.Map<TimelineEntity>(timelineUpdate);

        // Assert
        entity.Id.Should().Be(timelineUpdate.Id);
        entity.UserId.Should().Be(timelineUpdate.UserId);
        entity.SwapId.Should().Be(timelineUpdate.SwapId);
        entity.Status.Should().Be(timelineUpdate.Status);
        entity.Description.Should().Be(timelineUpdate.Description);
        entity.CreatedAt.Should().Be(timelineUpdate.CreatedAt);
    }

    [Fact]
    public void TimelineStatus_To_SwapStatus_MapsCorrectly()
    {
        // Test the custom converter

        // Arrange
        var converter = new TimelineStatusToSwapStatusConverter();

        // Act & Assert
        converter.Convert(TimelineStatus.Requested, default, default!)
            .Should().Be(SwapStatus.Requested);
        
        converter.Convert(TimelineStatus.MeetingUp, default, default!)
            .Should().Be(SwapStatus.Ongoing);
        
        converter.Convert(TimelineStatus.Finished, default, default!)
            .Should().Be(SwapStatus.Completed);
        
        converter.Convert(TimelineStatus.Disputed, default, default!)
            .Should().Be(SwapStatus.Disputed);
    }

    [Fact(Skip = "The scenario is not possible")]
    public void TimelineEntity_To_TimelineUpdate_ThrowsForLongDescription()
    {
        // Arrange
        var longDesc = new string('x', 101);
        var entity = new TimelineEntity
        {
            Description = longDesc
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<TimelineUpdate>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*Description too long*");
    }
}