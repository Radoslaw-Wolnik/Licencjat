namespace Tests.Domain.Helpers;

public static class StringExtensions
{
    public static string Repeat(this string value, int count)
    {
        if (count <= 0) return string.Empty;
        return string.Concat(Enumerable.Repeat(value, count));
    }
}