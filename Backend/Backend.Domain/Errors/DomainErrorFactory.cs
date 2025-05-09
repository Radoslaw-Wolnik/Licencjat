// Domain/Errors/DomainErrorFactory.cs
using Backend.Domain.Common;

namespace Backend.Domain.Errors;

public static class DomainErrorFactory
{
    public static DomainError NotFound(string entityName, object key)
    {
        var code    = $"{entityName}.NotFound";
        var message = key == null
            ? $"{entityName} not found"
            : $"{entityName} with id '{key}' was not found";

        var err = new DomainError(code, message, ErrorType.NotFound);
        err.Metadata["Entity"] = entityName;
        if (key != null) err.Metadata["Key"] = key;
        return err;
    }

    public static DomainError AlreadyExists(string entityName, string detail)
    {
        var code    = $"{entityName}.Exists";
        var message = detail ?? $"{entityName} already exists";
        return new DomainError(code, message, ErrorType.Conflict);
    }

    public static DomainError LimitReached(string entityName, string detail)
    {
        var code    = $"{entityName}.LimitReached";
        var message = detail ?? $"{entityName} limit reached";
        return new DomainError(code, message, ErrorType.Validation);
    }

    public static DomainError Invalid(string entityName, string context, ErrorType type = ErrorType.Validation)
    {
        var code    = $"{entityName}.Invalid{context}";
        var message = $"Invalid {context.ToLower()} for {entityName.ToLower()}";
        return new DomainError(code, message, type);
    }

    public static DomainError BadRequest(string code, string message)
        => new DomainError(code, message, ErrorType.BadRequest);

    public static DomainError Unauthorized(string code, string message)
        => new DomainError(code, message, ErrorType.Unauthorized);


    public static DomainError Forbidden(string code, string message)
        => new DomainError(code, message, ErrorType.Forbidden);

    public static DomainError StorageError(string context)
    {
        var code    = "Storage.Error";
        var message = context is null
            ? "An error occurred while accessing storage"
            : $"Storage error: {context}";
        return new DomainError(code, message, ErrorType.StorageError);
    }

    public static DomainError ServiceUnavailable(string context)
    {
        var code    = "Service.Unavailable";
        var message = context is null
            ? "Service is temporarily unavailable"
            : $"Service unavailable: {context}";
        return new DomainError(code, message, ErrorType.ServiceUnavailable);
    }
}

