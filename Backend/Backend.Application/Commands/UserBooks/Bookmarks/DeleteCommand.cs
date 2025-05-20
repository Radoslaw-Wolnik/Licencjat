using FluentResults;
using MediatR;

namespace Backend.Application.Commands.UserBooks.Bookmarks;

public sealed record DeleteBookmarkCommand(
    Guid BookmarkId
    ) : IRequest<Result>;