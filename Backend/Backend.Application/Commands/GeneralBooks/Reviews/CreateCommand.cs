using Backend.Domain.Common;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Reviews;

public sealed record CreateReviewCommand(
    Guid UserId,
    Guid BookId,
    int Rating,
    string? Comment
    ) : IRequest<Result<Guid>>;