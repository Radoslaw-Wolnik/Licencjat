using Backend.Application.ReadModels.Swaps;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Swaps;

public sealed record GetMeetupByIdQuery(
    Guid MeetupId
) : IRequest<Result<MeetupReadModel>>;