using System.Net;
using Backend.Domain.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult Match<T>(
        this Result<T> result,
        Func<T, IActionResult> onSuccess,
        Func<List<IError>, IActionResult> onFailure)
    {
        return result.IsSuccess 
            ? onSuccess(result.Value) 
            : onFailure(result.Errors);
    }

    public static ProblemDetails ToProblemDetails(
        this List<IError> errors, 
        string defaultTitle = "An error occurred")
    {
        var firstError = errors.FirstOrDefault();
        var statusCode = firstError switch
        {
            DomainError domainError => (int)MapErrorType(domainError.Type),
            _ => StatusCodes.Status400BadRequest
        };

        return new ProblemDetails
        {
            Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            Title = firstError?.Message ?? defaultTitle,
            Status = statusCode,
            Extensions = { ["errors"] = errors }
        };
    }

    private static HttpStatusCode MapErrorType(ErrorType type) => type switch
    {
        ErrorType.Validation => HttpStatusCode.BadRequest,
        ErrorType.Conflict => HttpStatusCode.Conflict,
        ErrorType.NotFound => HttpStatusCode.NotFound,
        _ => HttpStatusCode.InternalServerError
    };
}