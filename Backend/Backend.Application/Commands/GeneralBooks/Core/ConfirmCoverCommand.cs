using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Core;

public sealed record ConfirmGBCoverCommand(
    Guid BookId,
    string ImageObjectKey
    ) : IRequest<Result>;