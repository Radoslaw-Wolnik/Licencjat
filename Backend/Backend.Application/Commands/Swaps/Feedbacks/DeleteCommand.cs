using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Feedbacks;

public sealed record DeleteCommand(
    Guid FeedbackId
    ) : IRequest<Result>;