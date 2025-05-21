using Backend.Application.Interfaces.Repositories;
using Backend.Domain.Entities;
using Backend.Domain.Errors;
using FluentResults;
using MediatR;
using Backend.Application.Interfaces;
using Backend.Domain.Common;
using Backend.Domain.Enums;
using Backend.Application.Interfaces.DbReads;

namespace Backend.Application.Commands.Users.Core;
public class DeleteUserCommandHandler
    : IRequestHandler<DeleteUserCommand, Result>
{
    private readonly IWriteUserRepository _userRepo;
    private readonly IUserReadService _userRead;

    public DeleteUserCommandHandler(
        IWriteUserRepository userRepository,
        IUserReadService userReadService)
    {
        _userRepo = userRepository;
        _userRead = userReadService;
    }

    public async Task<Result> Handle(
        DeleteUserCommand request,
        CancellationToken cancellationToken)
    {
        return await _userRepo.DeleteAsync(request.UserId, cancellationToken);
    }
}
