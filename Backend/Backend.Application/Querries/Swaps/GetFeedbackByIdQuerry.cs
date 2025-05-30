using Backend.Application.ReadModels.Swaps;
using FluentResults;
using MediatR;

namespace Backend.Application.Querries.Swaps;

public sealed record GetFeedbackByIdQuery(
    Guid FeedbackId
) : IRequest<Result<FeedbackReadModel>>;