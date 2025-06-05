using AutoMapper;
using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Mapping;
using FluentAssertions;

namespace Tests.Infrastructure.Mapping;

public class MeetupProfileTests
{
    private readonly IMapper _mapper;

    public MeetupProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<MeetupProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void MeetupEntity_To_Meetup_MapsCorrectly()
    {
        // Arrange
        var entity = new MeetupEntity
        {
            Id = Guid.NewGuid(),
            Location_X = 40.7128,
            Location_Y = -74.0060f, // -74.00599670410156
            Status = MeetupStatus.Confirmed,
            SuggestedUserId = Guid.NewGuid(),
            SwapId = Guid.NewGuid()
        };

        // Act
        var meetup = _mapper.Map<Meetup>(entity);

        // Assert
        meetup.Id.Should().Be(entity.Id);
        meetup.SwapId.Should().Be(entity.SwapId);
        meetup.SuggestedUserId.Should().Be(entity.SuggestedUserId);
        meetup.Status.Should().Be(entity.Status);
        meetup.Location.Latitude.Should().Be(40.7128);
        meetup.Location.Longitude.Should().Be(-74.0060f); // i should change to double, float has some garbage
    }

    [Fact]
    public void Meetup_To_MeetupEntity_MapsCorrectly()
    {
        // Arrange
        var meetup = new Meetup(
            Id: Guid.NewGuid(),
            SwapId: Guid.NewGuid(),
            SuggestedUserId: Guid.NewGuid(),
            Status: MeetupStatus.Proposed,
            Location: new LocationCoordinates(51.5074, -0.1278)
        );

        // Act
        var entity = _mapper.Map<MeetupEntity>(meetup);

        // Assert
        entity.Id.Should().Be(meetup.Id);
        entity.SwapId.Should().Be(meetup.SwapId);
        entity.SuggestedUserId.Should().Be(meetup.SuggestedUserId);
        entity.Status.Should().Be(meetup.Status);
        entity.Location_X.Should().Be(51.5074);
        entity.Location_Y.Should().Be(-0.1278f);
    }

    [Fact]
    public void MeetupEntity_To_Meetup_ThrowsForInvalidCoordinates()
    {
        // Arrange
        var entity = new MeetupEntity
        {
            Location_X = 100.0, // Invalid latitude
            Location_Y = 200.0f // Invalid longitude
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<Meetup>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*Invalid coordinates*");
    }

    [Fact]
    public void Meetup_To_MeetupEntity_HandlesStatusChanges()
    {
        // Arrange
        var meetup = new Meetup(
            Id: Guid.NewGuid(),
            SwapId: Guid.NewGuid(),
            SuggestedUserId: Guid.NewGuid(),
            Status: MeetupStatus.ChangedLocation,
            Location: new LocationCoordinates(35.6895, 139.6917)
        );

        // Act
        var entity = _mapper.Map<MeetupEntity>(meetup);

        // Assert
        entity.Status.Should().Be(MeetupStatus.ChangedLocation);
    }
}