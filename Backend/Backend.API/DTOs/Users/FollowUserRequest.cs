using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Users;

public sealed record FollowUserRequest(
    [Required] Guid UserFollowedId
);
