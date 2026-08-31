namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Represents a positive optimistic revision of one conversation state.
/// </summary>
public readonly record struct ConversationRevision :
    IComparable<ConversationRevision>
{
    private readonly long value;

    private ConversationRevision(long value)
    {
        this.value = value;
    }

    /// <summary>
    /// Gets the initial conversation revision.
    /// </summary>
    public static ConversationRevision Initial => new(1);

    /// <summary>
    /// Gets a value indicating whether this revision was initialized.
    /// </summary>
    public bool IsInitialized => value > 0;

    /// <summary>
    /// Gets the positive revision value.
    /// </summary>
    public long Value =>
        IsInitialized
            ? value
            : throw new InvalidOperationException(
                "The conversation revision is not initialized.");

    /// <summary>
    /// Creates a positive conversation revision.
    /// </summary>
    /// <param name="value">The positive revision value.</param>
    /// <returns>The validated revision.</returns>
    public static ConversationRevision From(long value)
    {
        if (!TryFrom(value, out var revision))
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                "Conversation revisions must be positive.");
        }

        return revision;
    }

    /// <summary>
    /// Attempts to create a positive conversation revision.
    /// </summary>
    /// <param name="value">The candidate revision.</param>
    /// <param name="revision">The validated revision when successful.</param>
    /// <returns><see langword="true"/> when the value is positive.</returns>
    public static bool TryFrom(
        long value,
        out ConversationRevision revision)
    {
        if (value <= 0)
        {
            revision = default;
            return false;
        }

        revision = new ConversationRevision(value);
        return true;
    }

    /// <summary>
    /// Returns the next checked revision.
    /// </summary>
    /// <returns>The next positive revision.</returns>
    public ConversationRevision Next() =>
        From(checked(Value + 1));

    /// <inheritdoc />
    public int CompareTo(ConversationRevision other) =>
        Value.CompareTo(other.Value);

    /// <inheritdoc />
    public override string ToString() =>
        IsInitialized ? value.ToString(
            global::System.Globalization.CultureInfo.InvariantCulture) :
            string.Empty;
}
