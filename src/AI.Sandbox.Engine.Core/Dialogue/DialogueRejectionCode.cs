namespace AI.Sandbox.Engine.Core.Dialogue;

/// <summary>
/// Represents one stable machine-readable dialogue-orchestration rejection code.
/// </summary>
public readonly record struct DialogueRejectionCode :
    IComparable<DialogueRejectionCode>
{
    private const int MaximumLength = 128;

    private DialogueRejectionCode(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the normalized code, or an empty string when uninitialized.
    /// </summary>
    public string Value { get; } = string.Empty;

    /// <summary>
    /// Gets a value indicating whether this code is initialized.
    /// </summary>
    public bool IsInitialized => Value.Length > 0;

    /// <summary>
    /// Parses one stable lower-case machine-readable code.
    /// </summary>
    /// <param name="text">The code text.</param>
    /// <returns>The initialized code.</returns>
    /// <exception cref="FormatException">The text is invalid.</exception>
    public static DialogueRejectionCode Parse(string text)
    {
        if (!IsValid(text))
        {
            throw new FormatException(
                "Dialogue rejection codes must use 1 through 128 lower-case " +
                "ASCII letters, digits, '.', '_' or '-'.");
        }

        return new DialogueRejectionCode(text);
    }

    /// <summary>
    /// Attempts to parse one stable machine-readable code.
    /// </summary>
    /// <param name="text">The candidate text.</param>
    /// <param name="code">The parsed code when valid.</param>
    /// <returns><see langword="true"/> when parsing succeeds.</returns>
    public static bool TryParse(
        string? text,
        out DialogueRejectionCode code)
    {
        if (!IsValid(text))
        {
            code = default;
            return false;
        }

        code = new DialogueRejectionCode(text!);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(DialogueRejectionCode other)
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
