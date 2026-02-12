using System.Text.RegularExpressions;
using PageBoostAI.Domain.Exceptions;

namespace PageBoostAI.Domain.ValueObjects;

public sealed partial class Email : IEquatable<Email>
{
    public string Value { get; }

    public Email(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !EmailRegex().IsMatch(value))
            throw new InvalidEmailException(value ?? string.Empty);

        Value = value.Trim().ToLowerInvariant();
    }

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled)]
    private static partial Regex EmailRegex();

    public bool Equals(Email? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is Email other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
    public override string ToString() => Value;

    public static bool operator ==(Email? left, Email? right) => Equals(left, right);
    public static bool operator !=(Email? left, Email? right) => !Equals(left, right);

    public static implicit operator string(Email email) => email.Value;
}
