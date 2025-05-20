using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.UserBooks.Core;

public sealed record CreateUserBookCommand(
    Guid UserId,
    Guid BookId,
    BookStatus Status,
    BookState State,
    string Language,
    int PageCount,
    string CoverFileName
    ) : IRequest<Result<(Guid, string)>>;