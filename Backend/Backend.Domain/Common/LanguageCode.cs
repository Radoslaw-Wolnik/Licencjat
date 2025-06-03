using Backend.Domain.Errors;
using FluentResults;
using System.Text.RegularExpressions;

namespace Backend.Domain.Common;

public sealed record LanguageCode
{
    public string Code { get; }

    private static readonly Regex ValidationRegex = new("^[a-z]{2,3}$");
    private static readonly HashSet<string> ValidCodes = new()
    {
        // ISO 639-1 codes
        "en", "pl", "de", "fr", "es", "it", "nl", "ja", "zh", "ru",
        "pt", "sv", "no", "da", "fi", "ar", "hi", "ko", "el", "he",
        // Add other codes
    };

    private LanguageCode(string code) => Code = code;

    public static Result<LanguageCode> Create(string code)
    {
        var errors = new List<IError>();
        var normalized = code.Trim().ToLower();

        if (string.IsNullOrWhiteSpace(normalized))
            return Result.Fail(DomainErrorFactory.Invalid("LanguageCode", "Language code cannot be empty"));
        
        if (!ValidationRegex.IsMatch(normalized))
            errors.Add(DomainErrorFactory.Invalid("LanguageCode", "Invalid language code format (must be 2-3 lowercase letters)"));
            
        if (!ValidCodes.Contains(normalized))
            errors.Add(DomainErrorFactory.Invalid("LanguageCode", "Unrecognized language code"));

        return errors.Count != 0
            ? Result.Fail(errors) 
            : new LanguageCode(normalized);
    }
}