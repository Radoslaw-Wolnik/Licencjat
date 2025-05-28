namespace Backend.Application.ReadModels.Users;
// currently used in registration command for exsists? function in the UserReadService
// and in unused function GetUserWithIncludes in user read service
// i woudl like this read model gone
public record UserProjection(
    Guid Id,
    string Email,
    string Username,
    string LocationCity,
    string LocationCountry,
    float Reputation
);