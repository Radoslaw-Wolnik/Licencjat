using Backend.Domain.Errors;
using FluentAssertions;
using FluentResults;

namespace Tests.Domain.Helpers;

public static class DomainErrorAssertions
{
    public static void ShouldBeNotFoundError(
        this IError error,
        string entityName,
        object? key = null)
    {
        error.Should().BeOfType<DomainError>();
        var domainError = (DomainError)error;

        domainError.Type.Should().Be(ErrorType.NotFound);
        domainError.Message.Should().Contain(entityName);
        domainError.Metadata["Entity"].Should().Be(entityName);

        if (key != null)
        {
            domainError.Message.Should().Contain(key.ToString());
            domainError.Metadata["Key"].Should().Be(key);
        }
    }

    public static void ShouldBeValidationError(
        this IError error,
        string entityName,
        string context)
    {
        error.Should().BeOfType<DomainError>();
        var domainError = (DomainError)error;

        domainError.Type.Should().Be(ErrorType.Validation);
        domainError.Message.Should().Contain(context.ToLower());
        domainError.Message.Should().Contain(entityName.ToLower());
    }
}