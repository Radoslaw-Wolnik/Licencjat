using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Meetups;

public sealed record CreateMeetupCommand(
    Guid SwapId,
    Guid UserId,
    double Latitude,
    double Longitude
    ) : IRequest<Result>; // or <Result<Meetup>> or <Result<Guid>>