using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Users;

public sealed record ConfirmProfilePictureRequest(
    [Required] string ImageObjectKey
);
