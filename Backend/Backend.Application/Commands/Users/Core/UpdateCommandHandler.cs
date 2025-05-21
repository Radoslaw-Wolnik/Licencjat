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
public class UpdateUserProfileCommandHandler
    : IRequestHandler<UpdateUserProfileCommand, Result<User>>
{
    private readonly IWriteUserRepository _userRepo;
    private readonly IUserReadService _userRead;

    public UpdateUserProfileCommandHandler(
        IWriteUserRepository userRepository,
        IUserReadService userReadService)
    {
        _userRepo = userRepository;
        _userRead = userReadService;
    }

    public async Task<Result<User>> Handle(
        UpdateUserProfileCommand request,
        CancellationToken cancellationToken)
    {
        // fetch the user
        var user = await _userRead.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
            return Result.Fail(DomainErrorFactory.NotFound("User", request.UserId));

        // update user
        if (request.Bio != null)
        {
            var bioResult = BioString.Create(request.Bio);
            if (bioResult.IsFailed)
                return Result.Fail(bioResult.Errors);
            user.UpdateBio(bioResult.Value);
        }

        // update user location
        if (request.City != null || request.CountryCode != null)
        {
            // decide what city and country we’re going to use
            var cityToUse = request.City ?? user.Location.City;
            var countryCodeToUseResult = request.CountryCode != null
                ? CountryCode.Create(request.CountryCode)
                : Result.Ok(user.Location.Country);

            if (countryCodeToUseResult.IsFailed)
                return Result.Fail(countryCodeToUseResult.Errors);

            // create & validate the new Location
            var locationResult = Location.Create(cityToUse, countryCodeToUseResult.Value);
            if (locationResult.IsFailed)
                return Result.Fail(locationResult.Errors);
            
            user.UpdateLocation(locationResult.Value);
        }

        // persist changes
        var persistanceResult = await _userRepo.UpdateProfileAsync(user, cancellationToken);
        if (persistanceResult.IsFailed)
            return Result.Fail(persistanceResult.Errors);

        return Result.Ok(user);
    }
}
