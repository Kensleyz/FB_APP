namespace PageBoostAI.Domain.ValueObjects;

public sealed class PostContent : IEquatable<PostContent>
{
    public const int MaxLength = 280;

    public string Text { get; }

    public PostContent(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Post content cannot be empty.", nameof(text));
        if (text.Length > MaxLength)
            throw new ArgumentException($"Post content cannot exceed {MaxLength} characters. Current length: {text.Length}.", nameof(text));

        Text = text;
    }

    public bool Equals(PostContent? other) => other is not null && Text == other.Text;
    public override bool Equals(object? obj) => obj is PostContent other && Equals(other);
    public override int GetHashCode() => Text.GetHashCode();
    public override string ToString() => Text;

    public static implicit operator string(PostContent content) => content.Text;
}
