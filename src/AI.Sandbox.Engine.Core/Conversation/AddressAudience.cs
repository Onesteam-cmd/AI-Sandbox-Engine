namespace AI.Sandbox.Engine.Core.Conversation;

/// <summary>
/// Represents one immutable resolved response audience.
/// </summary>
public sealed class AddressAudience
{
    private const int MaximumTargetCount = 64;
    private readonly global::System.Collections.ObjectModel.ReadOnlyCollection<
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>
        targetEntityIds;

    private AddressAudience(
        AddressAudienceKind kind,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>[] targets)
    {
        Kind = kind;
        targetEntityIds = Array.AsReadOnly(targets);
    }

    /// <summary>
    /// Gets the semantic audience kind.
    /// </summary>
    public AddressAudienceKind Kind { get; }

    /// <summary>
    /// Gets deterministically ordered target entity IDs.
    /// </summary>
    public IReadOnlyList<
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>
        TargetEntityIds => targetEntityIds;

    /// <summary>
    /// Creates an audience with no selected response addressee.
    /// </summary>
    /// <returns>The empty audience.</returns>
    public static AddressAudience None() =>
        new(AddressAudienceKind.None, Array.Empty<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>());

    /// <summary>
    /// Creates an audience containing one or more specific participants.
    /// </summary>
    /// <param name="targetEntityIds">The selected participant IDs.</param>
    /// <returns>The validated immutable audience.</returns>
    public static AddressAudience SpecificParticipants(
        IEnumerable<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>
            targetEntityIds) =>
        CreateTargets(
            AddressAudienceKind.SpecificParticipants,
            targetEntityIds);

    /// <summary>
    /// Creates an audience containing every eligible participant.
    /// </summary>
    /// <param name="targetEntityIds">
    /// Every eligible participant except the speaker.
    /// </param>
    /// <returns>The validated immutable audience.</returns>
    public static AddressAudience AllParticipants(
        IEnumerable<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>
            targetEntityIds) =>
        CreateTargets(AddressAudienceKind.AllParticipants, targetEntityIds);

    private static AddressAudience CreateTargets(
        AddressAudienceKind kind,
        IEnumerable<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                global::AI.Sandbox.Engine.Core.Entities.EntityIdKind>>
            source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var targets = source.ToArray();
        if (targets.Length is < 1 or > MaximumTargetCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(source),
                "Address audiences must contain from 1 through 64 targets.");
        }

        foreach (var target in targets)
        {
            if (target.IsEmpty)
            {
                throw new ArgumentException(
                    "Address audience target IDs must be non-empty.",
                    nameof(source));
            }
        }

        var ordered = targets.OrderBy(static target => target).ToArray();
        for (var index = 1; index < ordered.Length; index++)
        {
            if (ordered[index] == ordered[index - 1])
            {
                throw new ArgumentException(
                    "Address audience target IDs must be unique.",
                    nameof(source));
            }
        }

        return new AddressAudience(kind, ordered);
    }
}
