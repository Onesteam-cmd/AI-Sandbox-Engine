namespace AI.Sandbox.Engine.Core.Persistence;

/// <summary>
/// Represents a stable, transport-independent persistence schema identifier.
/// </summary>
/// <remarks>
/// Schema IDs are lowercase dot-separated names such as
/// <c>game.world</c> or <c>component.position</c>. CLR type names and assembly
/// names must not be used as persistent identity.
/// </remarks>
public readonly record struct PersistenceSchemaId :
    IComparable<PersistenceSchemaId>
{
    private const int MaximumLength = 128;
    private readonly string? value;

    private PersistenceSchemaId(string value)
    {
        this.value = value;
    }

    /// <summary>
    /// Gets the canonical schema text, or an empty string for the default value.
    /// </summary>
    public string Value => value ?? string.Empty;

    /// <summary>
    /// Gets a value indicating whether this is the uninitialized default value.
    /// </summary>
    public bool IsEmpty => value is null;

    /// <summary>
    /// Parses a canonical stable schema identifier.
    /// </summary>
    /// <param name="value">The lowercase dot-separated schema text.</param>
    /// <returns>The parsed schema identifier.</returns>
    public static PersistenceSchemaId Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!TryParse(value, out var result))
        {
            throw new FormatException(
                "Schema IDs must contain lowercase letter-led segments " +
                "separated by dots.");
        }

        return result;
    }

    /// <summary>
    /// Attempts to parse a canonical stable schema identifier.
    /// </summary>
    /// <param name="value">The schema text.</param>
    /// <param name="result">
    /// The parsed schema ID, or the default value when invalid.
    /// </param>
    /// <returns><see langword="true"/> when parsing succeeds.</returns>
    public static bool TryParse(
        string? value,
        out PersistenceSchemaId result)
    {
        if (string.IsNullOrEmpty(value) ||
            value.Length > MaximumLength)
        {
            result = default;
            return false;
        }

        var atSegmentStart = true;

        foreach (var character in value)
        {
            if (character == '.')
            {
                if (atSegmentStart)
                {
                    result = default;
                    return false;
                }

                atSegmentStart = true;
                continue;
            }

            if (atSegmentStart)
            {
                if (character is < 'a' or > 'z')
                {
                    result = default;
                    return false;
                }

                atSegmentStart = false;
                continue;
            }

            var isLowercaseLetter = character is >= 'a' and <= 'z';
            var isDigit = character is >= '0' and <= '9';
            var isHyphen = character == '-';

            if (!isLowercaseLetter && !isDigit && !isHyphen)
            {
                result = default;
                return false;
            }
        }

        if (atSegmentStart)
        {
            result = default;
            return false;
        }

        result = new PersistenceSchemaId(value);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(PersistenceSchemaId other)
    {
        return StringComparer.Ordinal.Compare(Value, other.Value);
    }

    /// <summary>
    /// Returns the canonical schema identifier.
    /// </summary>
    /// <returns>The canonical schema text.</returns>
    public override string ToString()
    {
        return Value;
    }
}
