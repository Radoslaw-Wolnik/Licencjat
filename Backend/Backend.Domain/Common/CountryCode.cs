using Backend.Domain.Errors;
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

    public CountryCode(string code) { 
        Code = code;
    }
    
    public static Result<CountryCode> Create(string code)
    {
        var errors = new List<IError>();
        var normalized = code.Trim().ToUpper();

        if (string.IsNullOrWhiteSpace(normalized))
            return Result.Fail(DomainErrorFactory.Invalid("CountryCode", "Country code was empty"));
        
        if (!ValidationRegex.IsMatch(normalized))
            errors.Add(DomainErrorFactory.Invalid("Country Code", "Invalid country code format (must be 2-3 uppercase letters)"));
            
        if (!ValidCodes.Contains(normalized))
            errors.Add(DomainErrorFactory.NotFound("CountryCode", normalized));

        return errors.Count != 0
            ? Result.Fail(errors) 
            : new CountryCode(normalized);
    }

    public static CountryCode FromCode(string code) => new(code);
}