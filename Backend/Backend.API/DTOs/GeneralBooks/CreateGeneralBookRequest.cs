using System.ComponentModel.DataAnnotations;

namespace Backend.API.DTOs.GeneralBooks;

public sealed record CreateGeneralBookRequest(
    [Required] string Title,
    [Required] string Author,
    [Required] DateOnly Published,
    [Required] string OriginalLanguage,
    [Required] string CoverFileName
);