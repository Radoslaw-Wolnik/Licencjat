namespace Backend.API.DTOs.UserBooks.Responses;

public sealed record UserBookDetailsResponse(
    Guid Id,
    // mby also geenralbookId?
    string Title,
    string Author,
    string LanguageCode,
    int PageCount,
    string CoverPhotoUrl,
    string State,
    string Status
);