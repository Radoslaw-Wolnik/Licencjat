using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Reviews;

public sealed record UpdateReviewCommand(
    Guid ReviewId,
    int? Rating,
    string? Comment
    ) : IRequest<Result>;