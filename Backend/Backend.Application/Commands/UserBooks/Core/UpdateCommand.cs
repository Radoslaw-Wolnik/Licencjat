using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.UserBooks.Core;

public sealed record UpdateUserBookCommand(
    Guid UserBookId,
    BookStatus? Status,
    BookState? State
    // string? Language not allowed, 
    // int? PageCount,
    ) : IRequest<Result<UserBook>>;