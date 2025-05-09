using Backend.Domain.Common;
using Backend.Domain.Entities;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.GeneralBooks.Core;

public sealed record ConfirmCoverCommand(
    Guid BookId,
    string ImageObjectKey
    ) : IRequest<Result>;