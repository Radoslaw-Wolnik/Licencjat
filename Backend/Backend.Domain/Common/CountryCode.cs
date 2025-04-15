// Backend.Domain/Common/CountryCode.cs
using FluentResults;
using System.Text.RegularExpressions;

namespace Backend.Domain.Common;

public sealed record CountryCode
{
    private static readonly Regex ValidationRegex = new("^[A-Z]{2,3}$");
    private static readonly HashSet<string> ValidCodes = new()
    {
        // ISO 3166-1 alpha-2 codes
        "US", "GB", "PL", "DE", "FR", "ES", "IT", "NL", "JP", "CN",
        "CA", "AU", "BR", "IN", "RU", "CH", "SE", "NO", "DK", "FI",
        // Add other codes
    };

    public string Code { get; }

    private CountryCode(string code) => Code = code;

    public static Result<CountryCode> Create(string code)
    {
        var errors = new List<IError>();
        var normalized = code.Trim().ToUpper();

        if (string.IsNullOrWhiteSpace(normalized))
            errors.Add(new Error("Country code cannot be empty"));
        
        if (!ValidationRegex.IsMatch(normalized))
            errors.Add(new Error("Invalid country code format (must be 2-3 uppercase letters)"));
            
        if (!ValidCodes.Contains(normalized))
            errors.Add(new Error("Unrecognized country code"));

        return errors.Any() 
            ? Result.Fail(errors) 
            : new CountryCode(normalized);
    }
}