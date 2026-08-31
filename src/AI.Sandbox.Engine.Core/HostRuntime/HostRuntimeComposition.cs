namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Contains one immutable deterministic host-runtime capability order.</summary>
public sealed record HostRuntimeComposition
{
    private readonly global::System.Collections.ObjectModel.ReadOnlyCollection<
        HostRuntimeCapabilityDescriptor> capabilities;

    internal HostRuntimeComposition(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeCompositionIdKind> compositionId,
        HostRuntimeCapabilityDescriptor[] capabilities)
    {
        CompositionId = compositionId;
        this.capabilities = Array.AsReadOnly(capabilities);
    }

    /// <summary>Gets the externally assigned stable composition ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeCompositionIdKind> CompositionId { get; }

    /// <summary>Gets capabilities in deterministic dependency-safe order.</summary>
    public IReadOnlyList<HostRuntimeCapabilityDescriptor> Capabilities =>
        capabilities;
}
