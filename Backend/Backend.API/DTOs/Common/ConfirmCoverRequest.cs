using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Common;

public sealed record ConfirmCoverRequest(
    [Required] string ImageObjectKey
);