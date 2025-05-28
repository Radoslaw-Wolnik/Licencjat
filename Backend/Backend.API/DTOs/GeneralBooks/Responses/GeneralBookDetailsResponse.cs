using Backend.API.DTOs.Common;

namespace Backend.API.DTOs.GeneralBooks.Responses;

public sealed record GeneralBookDetailsResponse(
    Guid Id,
    string Title,
    string Author,
    DateOnly Published,
    string LanguageCode,
    double RatingAvg,
    string CoverPhotoUrl,
    IEnumerable<string> Genres,
    PaginatedResponse<ReviewResponse> Reviews
);