namespace Backend.Domain.Enums;

public enum StorageDestination
{
    Users,        // maps to "users"
    UserBooks,    // maps to "userbooks"
    GeneralBooks  // maps to "generalbooks"
}

public static class StorageDestinationExtensions
{
    public static string ToPath(this StorageDestination dest)
        => dest.ToString().ToLowerInvariant();
}
