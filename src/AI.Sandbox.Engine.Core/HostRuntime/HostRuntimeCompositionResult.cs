namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Represents one explicit host-runtime composition result.</summary>
public sealed record HostRuntimeCompositionResult
{
    internal HostRuntimeCompositionResult(
        HostRuntimeCompositionStatus status,
        HostRuntimeComposition? composition,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeCapabilityIdKind> relatedCapabilityId)
    {
        Status = status;
        Composition = composition;
        RelatedCapabilityId = relatedCapabilityId;
    }

    /// <summary>Gets the explicit composition outcome.</summary>
    public HostRuntimeCompositionStatus Status { get; }

    /// <summary>Gets the immutable composition when composition succeeded.</summary>
    public HostRuntimeComposition? Composition { get; }

    /// <summary>
    /// Gets the capability related to a duplicate, missing dependency, or cycle.
    /// </summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeCapabilityIdKind> RelatedCapabilityId { get; }

    /// <summary>Gets whether a usable immutable composition was produced.</summary>
    public bool Succeeded =>
        Status == HostRuntimeCompositionStatus.Composed &&
        Composition is not null;
}
