namespace AI.Sandbox.Engine.Core.Social;

/// <summary>
/// Represents a stable machine-readable coordination rejection code.
/// </summary>
public readonly record struct SocialTurnRejectionCode :
    IComparable<SocialTurnRejectionCode>
{
    private const int MaximumLength = 96;
    private readonly string? value;

    private SocialTurnRejectionCode(string value)
    {
        this.value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this code was initialized.
    /// </summary>
    public bool IsInitialized => value is not null;

    /// <summary>
    /// Gets the validated ordinal code value.
    /// </summary>
    public string Value =>
        value ??
        throw new InvalidOperationException(
            "The social turn code is not initialized.");

    /// <summary>
    /// Creates a stable machine-readable code.
    /// </summary>
    /// <param name="value">
    /// A lowercase code containing letters, digits, dots, underscores, or
    /// hyphens.
    /// </param>
    /// <returns>The validated code.</returns>
    public static SocialTurnRejectionCode From(string value)
    {
        if (!TryFrom(value, out var code))
        {
            throw new ArgumentException(
                "Social turn codes must be non-empty lowercase " +
                "machine-readable values no longer than 96 characters.",
                nameof(value));
        }

        return code;
    }

    /// <summary>
    /// Attempts to create a stable machine-readable code.
    /// </summary>
    /// <param name="value">The candidate code.</param>
    /// <param name="code">The validated code when successful.</param>
    /// <returns><see langword="true"/> when the code is valid.</returns>
    public static bool TryFrom(string? value, out SocialTurnRejectionCode code)
    {
        if (!IsValid(value))
        {
            code = default;
            return false;
        }

        code = new SocialTurnRejectionCode(value!);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(SocialTurnRejectionCode other)
    {
        if (!IsInitialized)
        {
            return other.IsInitialized ? -1 : 0;
        }

        if (!other.IsInitialized)
        {
            return 1;
        }

        return string.CompareOrdinal(Value, other.Value);
    }

    /// <inheritdoc />
    public override string ToString() =>
        IsInitialized ? Value : string.Empty;

    private static bool IsValid(string? text)
    {
        if (string.IsNullOrEmpty(text) || text.Length > MaximumLength)
        {
            return false;
        }

        foreach (var character in text)
        {
            var valid =
                character is >= 'a' and <= 'z' ||
                character is >= '0' and <= '9' ||
                character is '.' or '_' or '-';

            if (!valid)
            {
                return false;
            }
        }

        return true;
    }
}
