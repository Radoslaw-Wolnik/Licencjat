// Domain/Common/DomainError.cs
using FluentResults;

namespace Backend.Domain.Common;

public record DomainError(
    string Code, 
    string Message, 
    ErrorType Type
) : IError  // Implement FluentResults' IError
{
    public List<IError> Reasons { get; } = new();  // Required by IError
    public Dictionary<string, object> Metadata { get; } = new();  // Required by IError
}

public enum ErrorType { Validation, Conflict, Unauthorized, NotFound, Forbidden, StorageError, ServiceUnavailable }