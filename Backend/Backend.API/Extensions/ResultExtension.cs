using System.Net;
using Backend.Domain.Errors;
using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Backend.API.Extensions;

public static class ResultExtensions
{
    
    // For Result<T>
    public static IActionResult Match<T>(
        this Result<T> result,
        Func<T, IActionResult> onSuccess,
        Func<List<IError>, IActionResult> onFailure)
    {
        return result.IsSuccess
            ? onSuccess(result.Value)
            : onFailure(result.Errors);
    }

    // For non-generic Result
    public static IActionResult Match(
        this Result result,
        Func<IActionResult> onSuccess,
        Func<List<IError>, IActionResult> onFailure)
    {
        return result.IsSuccess
            ? onSuccess()
            : onFailure(result.Errors);
    }

    public static IActionResult ToProblemDetailsResult(this List<IError> errors)
    {
        var problemDetails = new ProblemDetails
        {
            Type = "https://httpstatuses.io/400",
            Title = "Request processing error",
            Status = StatusCodes.Status400BadRequest
        };

        // Initialize errors dictionary
        var errorDetails = new Dictionary<string, object>();
        problemDetails.Extensions["errors"] = errorDetails;

        // Set status based on first domain error
        var domainError = errors.OfType<DomainError>().FirstOrDefault();
        if (domainError != null)
        {
            problemDetails.Status = (int)MapErrorType(domainError.Type);
            problemDetails.Title = domainError.Message;
        }

        // Collect all error metadata
        foreach (var error in errors)
        {
            // For DomainErrors, use their custom properties
            if (error is DomainError dError)
            {
                errorDetails[dError.Code ?? "DOMAIN_ERROR"] = new
                {
                    dError.Message,
                    dError.Type,
                    dError.Code
                };
            }
            // For general errors, use message and metadata
            else
            {
                var key = error.Message;
                if (error.Metadata.TryGetValue("Code", out var code) && code is string codeStr)
                {
                    key = codeStr;
                }

                errorDetails[key] = new
                {
                    error.Message,
                    Metadata = error.Metadata
                };
            }
        }

        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }

    private static HttpStatusCode MapErrorType(ErrorType type) => type switch
    {
        ErrorType.Validation => HttpStatusCode.BadRequest,
        ErrorType.Conflict => HttpStatusCode.Conflict,
        ErrorType.NotFound => HttpStatusCode.NotFound,
        ErrorType.Unauthorized => HttpStatusCode.Unauthorized,
        ErrorType.Forbidden => HttpStatusCode.Forbidden,
        _ => HttpStatusCode.InternalServerError
    };
}