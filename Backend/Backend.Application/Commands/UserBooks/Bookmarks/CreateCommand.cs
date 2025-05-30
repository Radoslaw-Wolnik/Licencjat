using Backend.Domain.Common;
using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.UserBooks.Bookmarks;

public sealed record CreateBookmarkCommand(
    Guid UserBookId,
    BookmarkColours Colour,
    int Page,
    string? Description
    ) : IRequest<Result<Guid>>;