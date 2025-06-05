using AutoMapper;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Infrastructure.Entities;
using Backend.Infrastructure.Mapping;
using FluentAssertions;

namespace Tests.Infrastructure.Mapping;

public class FeedbackProfileTests
{
    private readonly IMapper _mapper;

    public FeedbackProfileTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<FeedbackProfile>());
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void FeedbackEntity_To_Feedback_MapsCorrectly()
    {
        // Arrange
        var entity = new FeedbackEntity
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            SubSwapId = Guid.NewGuid(),
            Stars = 4,
            Recommend = true,
            Lenght = SwapLength.JustRight,
            ConditionBook = SwapConditionBook.Same,
            Communication = SwapCommunication.Perfect
        };

        // Act
        var feedback = _mapper.Map<Feedback>(entity);

        // Assert
        feedback.Id.Should().Be(entity.Id);
        feedback.UserId.Should().Be(entity.UserId);
        feedback.SubSwapId.Should().Be(entity.SubSwapId);
        feedback.Stars.Should().Be(entity.Stars);
        feedback.Recommend.Should().Be(entity.Recommend);
        feedback.Length.Should().Be(entity.Lenght);
        feedback.Condition.Should().Be(entity.ConditionBook);
        feedback.Communication.Should().Be(entity.Communication);
    }

    [Fact]
    public void Feedback_To_FeedbackEntity_MapsCorrectly()
    {
        // Arrange
        var feedback = new Feedback(
            Id: Guid.NewGuid(),
            SubSwapId: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            Stars: 5,
            Recommend: true,
            Length: SwapLength.TooShort,
            Condition: SwapConditionBook.Better,
            Communication: SwapCommunication.Okay
        );

        // Act
        var entity = _mapper.Map<FeedbackEntity>(feedback);

        // Assert
        entity.Id.Should().Be(feedback.Id);
        entity.UserId.Should().Be(feedback.UserId);
        entity.SubSwapId.Should().Be(feedback.SubSwapId);
        entity.Stars.Should().Be(feedback.Stars);
        entity.Recommend.Should().Be(feedback.Recommend);
        entity.Lenght.Should().Be(feedback.Length);
        entity.ConditionBook.Should().Be(feedback.Condition);
        entity.Communication.Should().Be(feedback.Communication);
    }

    [Fact]
    public void FeedbackEntity_To_Feedback_ThrowsForInvalidStars()
    {
        // Arrange
        var entity = new FeedbackEntity
        {
            Stars = 6 // Invalid
        };

        // Act & Assert
        _mapper.Invoking(m => m.Map<Feedback>(entity))
            .Should().Throw<AutoMapperMappingException>()
            .WithMessage("*Stars must be between 1 and 5*");
    }
}