namespace AI.Sandbox.Engine.Core.Behavior;

/// <summary>
/// Represents one stable machine-readable reason for rejecting an action.
/// </summary>
public readonly record struct ActionRejectionCode :
    IComparable<ActionRejectionCode>
{
    private const int MaximumLength = 64;
    private readonly string? value;

    private ActionRejectionCode(string value)
    {
        this.value = value;
    }

    /// <summary>
    /// Gets a value indicating whether this code was initialized.
    /// </summary>
    public bool IsInitialized => value is not null;

    /// <summary>
    /// Gets the validated ordinal rejection-code text.
    /// </summary>
    public string Value =>
        value ??
        throw new InvalidOperationException(
            "The action rejection code is not initialized.");

    /// <summary>
    /// Parses one stable lowercase rejection code.
    /// </summary>
    /// <param name="text">The code text.</param>
    /// <returns>The validated code.</returns>
    public static ActionRejectionCode Parse(string text)
    {
        if (!TryParse(text, out var code))
        {
            throw new FormatException(
                "Action rejection codes must contain 1 to 64 lowercase ASCII " +
                "letters, digits, periods, underscores, or hyphens.");
        }

        return code;
    }

    /// <summary>
    /// Attempts to parse one stable lowercase rejection code.
    /// </summary>
    /// <param name="text">The candidate code text.</param>
    /// <param name="code">The parsed code when valid.</param>
    /// <returns><see langword="true"/> when parsing succeeds.</returns>
    public static bool TryParse(
        string? text,
        out ActionRejectionCode code)
    {
        if (!IsValid(text))
        {
            code = default;
            return false;
        }

        code = new ActionRejectionCode(text!);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(ActionRejectionCode other)
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
