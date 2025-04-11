// Domain/Errors/GeneralErrors.cs
using Backend.Domain.Common;
namespace Backend.Domain.Errors;

public static class DatabaseErrors
{
    public static DomainError StorageError => new(
        "General.StorageError",
        "Error accessing data storage",
        ErrorType.StorageError);

    public static DomainError EntityNotFound => new(
        "General.NotFound",
        "Requested entity not found",
        ErrorType.NotFound);
}