using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Reviews;

public sealed record DeleteReviewCommand(
    // Guid BookId,
    Guid ReviewId
    ) : IRequest<Result>;