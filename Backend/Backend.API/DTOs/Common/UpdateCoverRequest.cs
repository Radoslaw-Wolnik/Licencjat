using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Common;

public sealed record UpdateCoverRequest(
    [Required] string CoverFileName
);