using Backend.Domain.Common;
using Backend.Domain.Entities;
using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.UserBooks.Core;

public sealed record UpdateCommand(
    Guid UserBookId,
    BookStatus? Status,
    BookState? State,
    LanguageCode? Language,
    int? PageCount,
    Photo? CoverPhoto
    ) : IRequest<Result<UserBook>>;