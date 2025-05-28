using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Users;

public sealed record UpdateProfilePictureRequest(
    [Required] string ProfilePictureFileName
);
