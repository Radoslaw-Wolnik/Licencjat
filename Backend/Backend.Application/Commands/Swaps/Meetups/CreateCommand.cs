using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Meetups;

public sealed record CreateCommand(
    Guid SwapId,
    Guid UserId,
    LocationCoordinates Location
    ) : IRequest<Result<Meetup>>;