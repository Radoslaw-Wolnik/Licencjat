using FluentResults;
using MediatR;

namespace Backend.Application.Commands.Swaps.Issues;

public sealed record CreateCommand(
    Guid SubSwapId, // or Swap Id
    Guid UserId,
    string Description
    ) : IRequest<Result>; // <Result<Issue>>