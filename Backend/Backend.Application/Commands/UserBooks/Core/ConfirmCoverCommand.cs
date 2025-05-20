using FluentResults;
using MediatR;

namespace Backend.Application.Commands.UserBooks.Core;

public sealed record ConfirmUBCoverCommand(
    Guid UserBookId,
    string ImageObjectKey
    ) : IRequest<Result>;