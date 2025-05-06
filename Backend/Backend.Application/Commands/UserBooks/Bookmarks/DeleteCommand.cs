using Backend.Domain.Common;
using Backend.Domain.Enums;
using FluentResults;
using MediatR;

namespace Backend.Application.Commands.UserBooks.Bookmarks;

public sealed record DeleteCommand(
    Guid BookmarkId
    ) : IRequest<Result>;