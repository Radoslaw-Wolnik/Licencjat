using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Users;

public sealed record BlockUserRequest(
    [Required] Guid UserBlockedId
);
