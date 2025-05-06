using Backend.Domain.Common;
using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.UserBooks.Core;

public sealed record DeleteCommand(
    Guid BookId
    ) : IRequest<Result>;