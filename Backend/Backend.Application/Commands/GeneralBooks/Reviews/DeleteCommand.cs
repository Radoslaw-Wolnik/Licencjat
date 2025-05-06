using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Reviews;

public sealed record DeleteCommand(
    Guid ReviewId
    ) : IRequest<Result>;