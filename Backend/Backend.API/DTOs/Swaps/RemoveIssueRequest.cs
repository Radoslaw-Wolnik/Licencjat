using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.Swaps;

public sealed record RemoveIssueRequest(
    [Required] string ResolutionDetails
);