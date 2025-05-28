using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Swaps;

public sealed record AddIssueRequest(
    [Required] string Description
);
