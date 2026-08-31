namespace AI.Sandbox.Engine.Core.Identifiers;

/// <summary>
/// Represents a strongly typed, GUID-backed identifier whose kind is encoded by
/// the generic type argument.
/// </summary>
/// <typeparam name="TKind">
/// A compile-time marker type that distinguishes this identifier from identifiers
/// belonging to other engine concepts. No instance of <typeparamref name="TKind"/>
/// is created or stored.
/// </typeparam>
/// <remarks>
/// The default value is empty and must be treated as uninitialized. Valid
/// identifiers are created from an externally supplied, non-empty
/// <see cref="Guid"/>. Identifier generation is intentionally outside this type
/// so deterministic simulations can control the source of new values.
/// </remarks>
public readonly record struct Id<TKind> : IComparable<Id<TKind>>
{
    private const string CanonicalFormat = "D";

    private Id(Guid value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the underlying GUID value.
    /// </summary>
    public Guid Value { get; }

    /// <summary>
    /// Gets a value indicating whether this identifier is the uninitialized
    /// default value.
    /// </summary>
    public bool IsEmpty => Value == Guid.Empty;

    /// <summary>
    /// Creates a typed identifier from a non-empty GUID supplied by an external
    /// identity source.
    /// </summary>
    /// <param name="value">The non-empty GUID to wrap.</param>
    /// <returns>A typed identifier containing <paramref name="value"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="value"/> is <see cref="Guid.Empty"/>.
    /// </exception>
    public static Id<TKind> From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException(
                "A valid identifier cannot contain Guid.Empty.",
                nameof(value));
        }

        return new Id<TKind>(value);
    }

    /// <summary>
    /// Attempts to create a typed identifier from a GUID.
    /// </summary>
    /// <param name="value">The GUID to validate and wrap.</param>
    /// <param name="result">
    /// The resulting identifier when successful; otherwise the empty default value.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when <paramref name="value"/> is non-empty;
    /// otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryFrom(Guid value, out Id<TKind> result)
    {
        if (value == Guid.Empty)
        {
            result = default;
            return false;
        }

        result = new Id<TKind>(value);
        return true;
    }

    /// <summary>
    /// Parses a canonical GUID string into a typed identifier.
    /// </summary>
    /// <param name="value">
    /// A non-empty GUID in canonical <c>8-4-4-4-12</c> form.
    /// </param>
    /// <returns>The parsed typed identifier.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="FormatException">
    /// Thrown when the value is malformed, non-canonical, or represents
    /// <see cref="Guid.Empty"/>.
    /// </exception>
    public static Id<TKind> Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!TryParse(value, out var result))
        {
            throw new FormatException(
                "Identifier text must be a non-empty GUID in canonical D format.");
        }

        return result;
    }

    /// <summary>
    /// Attempts to parse a canonical GUID string into a typed identifier.
    /// </summary>
    /// <param name="value">
    /// A GUID in canonical <c>8-4-4-4-12</c> form.
    /// </param>
    /// <param name="result">
    /// The parsed identifier when successful; otherwise the empty default value.
    /// </param>
    /// <returns>
    /// <see langword="true"/> when the text is canonical and non-empty;
    /// otherwise <see langword="false"/>.
    /// </returns>
    public static bool TryParse(string? value, out Id<TKind> result)
    {
        if (!Guid.TryParseExact(value, CanonicalFormat, out var parsed) ||
            parsed == Guid.Empty)
        {
            result = default;
            return false;
        }

        result = new Id<TKind>(parsed);
        return true;
    }

    /// <inheritdoc />
    public int CompareTo(Id<TKind> other)
    {
        return Value.CompareTo(other.Value);
    }

    /// <summary>
    /// Returns the stable canonical GUID representation of this identifier.
    /// </summary>
    /// <returns>
    /// The underlying GUID formatted as lowercase-compatible canonical
    /// <c>8-4-4-4-12</c> text.
    /// </returns>
    public override string ToString()
    {
        return Value.ToString(CanonicalFormat);
    }
}
