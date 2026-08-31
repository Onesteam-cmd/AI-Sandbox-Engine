namespace AI.Sandbox.Engine.Core.Dialogue;

/// <summary>
/// Represents one positive host-assigned ordering value for a dialogue artifact.
/// </summary>
public readonly record struct DialogueArtifactSequence :
    IComparable<DialogueArtifactSequence>
{
    private DialogueArtifactSequence(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the positive sequence value, or zero when uninitialized.
    /// </summary>
    public int Value { get; }

    /// <summary>
    /// Gets a value indicating whether this sequence is initialized.
    /// </summary>
    public bool IsInitialized => Value > 0;

    /// <summary>
    /// Creates one positive artifact sequence.
    /// </summary>
    /// <param name="value">The positive ordering value.</param>
    /// <returns>The initialized sequence.</returns>
    public static DialogueArtifactSequence From(int value)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                "Dialogue artifact sequences must be positive.");
        }

        return new DialogueArtifactSequence(value);
    }

    /// <inheritdoc />
    public int CompareTo(DialogueArtifactSequence other) =>
        Value.CompareTo(other.Value);
}
