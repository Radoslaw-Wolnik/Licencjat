using FluentResults;
using MediatR;

namespace Backend.Application.Commands.UserBooks.Core;

public sealed record DeleteUserBookCommand(
    Guid UserBookId
    // string PhotoKey - insted we fetch it from the databse
    ) : IRequest<Result>;