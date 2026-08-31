namespace AI.Sandbox.Engine.Core.Prompting;

/// <summary>
/// Captures one immutable owner-scoped prompt candidate with deterministic
/// inclusion, priority, and estimated cost coordinates.
/// </summary>
/// <typeparam name="TContent">The exact candidate-content payload type.</typeparam>
public sealed record PromptCandidateEnvelope<TContent>
    where TContent : IPromptContent
{
    private PromptCandidateEnvelope(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptCandidateIdKind>
            candidateId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        PromptInclusionMode inclusionMode,
        PromptPriority priority,
        PromptCost cost,
        TContent payload)
    {
        CandidateId = candidateId;
        OwnerEntityId = ownerEntityId;
        WorldId = worldId;
        InclusionMode = inclusionMode;
        Priority = priority;
        Cost = cost;
        Payload = payload;
    }

    /// <summary>
    /// Gets the externally assigned stable candidate ID.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptCandidateIdKind>
        CandidateId { get; }

    /// <summary>
    /// Gets the subjective owner represented by this candidate.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> OwnerEntityId
        { get; }

    /// <summary>
    /// Gets the authoritative world represented by this candidate.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> WorldId
        { get; }

    /// <summary>
    /// Gets whether this candidate is required or optional.
    /// </summary>
    public PromptInclusionMode InclusionMode { get; }

    /// <summary>
    /// Gets deterministic host-assigned selection priority.
    /// </summary>
    public PromptPriority Priority { get; }

    /// <summary>
    /// Gets the positive provider-neutral estimated cost.
    /// </summary>
    public PromptCost Cost { get; }

    /// <summary>
    /// Gets the exact candidate-content payload.
    /// </summary>
    public TContent Payload { get; }

    /// <summary>
    /// Creates one immutable owner-scoped prompt candidate.
    /// </summary>
    /// <param name="candidateId">The externally assigned candidate ID.</param>
    /// <param name="ownerEntityId">The subjective owner entity.</param>
    /// <param name="worldId">The authoritative world ID.</param>
    /// <param name="inclusionMode">Required or optional inclusion.</param>
    /// <param name="priority">Deterministic host-assigned priority.</param>
    /// <param name="cost">Positive estimated cost.</param>
    /// <param name="payload">The exact candidate payload.</param>
    /// <returns>The validated immutable candidate.</returns>
    public static PromptCandidateEnvelope<TContent> Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<PromptCandidateIdKind>
            candidateId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.Entities.EntityIdKind> ownerEntityId,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            global::AI.Sandbox.Engine.Core.WorldState.WorldIdKind> worldId,
        PromptInclusionMode inclusionMode,
        PromptPriority priority,
        PromptCost cost,
        TContent payload)
    {
        PromptTypePolicy.EnsureExactType(
            typeof(TContent),
            typeof(IPromptContent),
            "prompt content");

        EnsureNonEmpty(candidateId, nameof(candidateId));
        EnsureNonEmpty(ownerEntityId, nameof(ownerEntityId));
        EnsureNonEmpty(worldId, nameof(worldId));

        if (!Enum.IsDefined(inclusionMode))
        {
            throw new ArgumentOutOfRangeException(
                nameof(inclusionMode),
                inclusionMode,
                "The prompt inclusion mode is unknown.");
        }

        if (!priority.IsInitialized)
        {
            throw new ArgumentException(
                "Prompt priority must be initialized.",
                nameof(priority));
        }

        if (!cost.IsInitialized)
        {
            throw new ArgumentException(
                "Prompt cost must be initialized.",
                nameof(cost));
        }

        if (payload is null)
        {
            throw new ArgumentNullException(nameof(payload));
        }

        return new PromptCandidateEnvelope<TContent>(
            candidateId,
            ownerEntityId,
            worldId,
            inclusionMode,
            priority,
            cost,
            payload);
    }

    private static void EnsureNonEmpty<TKind>(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<TKind> id,
        string parameterName)
        where TKind : struct
    {
        if (id.IsEmpty)
        {
            throw new ArgumentException(
                "Prompt identifiers cannot be empty.",
                parameterName);
        }
    }
}
