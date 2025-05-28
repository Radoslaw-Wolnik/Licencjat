using Backend.Domain.Common;
using Backend.Domain.Enums.SortBy;
using Backend.Application.ReadModels.Users;
using Backend.Application.ReadModels.Common;

namespace Backend.Application.Interfaces.Queries;

public interface IUserQueryService
{

    Task<PaginatedResult<UserSmallReadModel>> ListAsync(
        string UserName,
        float Reputation,

        string City,
        string Country,

        SortUsersBy sortBy,
        bool descending,
        int offset,
        int limit,
        CancellationToken ct = default
    );

    Task<UserProfileReadModel?> GetDetailsAsync(
        Guid userId,
        CancellationToken ct = default
    );

}
