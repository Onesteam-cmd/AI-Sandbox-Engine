namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Represents a positive sequential turn number inside one conversation.
/// </summary>
public readonly record struct ConversationTurnNumber :
    IComparable<ConversationTurnNumber>
{
    private readonly long value;

    private ConversationTurnNumber(long value)
    {
        this.value = value;
    }

    /// <summary>
    /// Gets the first conversation turn number.
    /// </summary>
    public static ConversationTurnNumber First => new(1);

    /// <summary>
    /// Gets a value indicating whether this turn number was initialized.
    /// </summary>
    public bool IsInitialized => value > 0;

    /// <summary>
    /// Gets the positive turn number.
    /// </summary>
    public long Value =>
        IsInitialized
            ? value
            : throw new InvalidOperationException(
                "The conversation turn number is not initialized.");

    /// <summary>
    /// Creates a positive conversation turn number.
    /// </summary>
    /// <param name="value">The positive turn number.</param>
    /// <returns>The validated turn number.</returns>
    public static ConversationTurnNumber From(long value)
    {
        if (!TryFrom(value, out var number))
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Conversation turn numbers must be positive.");
        }

        return number;
    }

    /// <summary>
    /// Attempts to create a positive conversation turn number.
    /// </summary>
    /// <param name="value">The candidate number.</param>
    /// <param name="number">The validated number when successful.</param>
    /// <returns><see langword="true"/> when the value is positive.</returns>
    public static bool TryFrom(
        long value,
        out ConversationTurnNumber number)
    {
        if (value <= 0)
        {
            number = default;
            return false;
        }

        number = new ConversationTurnNumber(value);
        return true;
    }

    /// <summary>
    /// Returns the next checked turn number.
    /// </summary>
    /// <returns>The next positive turn number.</returns>
    public ConversationTurnNumber Next() =>
        From(checked(Value + 1));

    /// <inheritdoc />
    public int CompareTo(ConversationTurnNumber other) =>
        Value.CompareTo(other.Value);

    /// <inheritdoc />
    public override string ToString() =>
        IsInitialized ? value.ToString(
            global::System.Globalization.CultureInfo.InvariantCulture) :
            string.Empty;
}
