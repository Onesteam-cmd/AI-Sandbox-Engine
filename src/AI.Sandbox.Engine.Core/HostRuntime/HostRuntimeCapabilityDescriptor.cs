namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Describes one exact host capability and its explicit capability dependencies.
/// </summary>
public sealed record HostRuntimeCapabilityDescriptor
{
    private const int MaximumDependencyCount = 32;
    private readonly global::System.Collections.ObjectModel.ReadOnlyCollection<
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeCapabilityIdKind>> dependencies;

    private HostRuntimeCapabilityDescriptor(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeCapabilityIdKind> capabilityId,
        IHostRuntimeCapability payload,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeCapabilityIdKind>[] dependencies)
    {
        CapabilityId = capabilityId;
        Payload = payload;
        this.dependencies = Array.AsReadOnly(dependencies);
    }

    /// <summary>Gets the externally assigned stable capability ID.</summary>
    public global::AI.Sandbox.Engine.Core.Identifiers.Id<
        HostRuntimeCapabilityIdKind> CapabilityId { get; }

    /// <summary>Gets the exact immutable capability payload.</summary>
    public IHostRuntimeCapability Payload { get; }

    /// <summary>Gets dependency IDs in stable ordinal order.</summary>
    public IReadOnlyList<
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeCapabilityIdKind>> Dependencies => dependencies;

    /// <summary>
    /// Creates one validated immutable capability descriptor.
    /// </summary>
    /// <param name="capabilityId">Externally assigned non-empty capability ID.</param>
    /// <param name="payload">Exact immutable capability payload.</param>
    /// <param name="dependencies">Explicit dependency capability IDs.</param>
    /// <returns>A validated immutable descriptor.</returns>
    public static HostRuntimeCapabilityDescriptor Create(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeCapabilityIdKind> capabilityId,
        IHostRuntimeCapability payload,
        IEnumerable<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeCapabilityIdKind>> dependencies)
    {
        if (capabilityId.IsEmpty)
        {
            throw new ArgumentException(
                "The capability ID must be initialized.",
                nameof(capabilityId));
        }

        HostRuntimeTypePolicy.EnsureExactCapability(payload);
        ArgumentNullException.ThrowIfNull(dependencies);

        var dependencyArray = dependencies.ToArray();
        if (dependencyArray.Length > MaximumDependencyCount)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dependencies),
                "A host capability may declare at most 32 dependencies.");
        }

        var seen = new HashSet<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeCapabilityIdKind>>();
        foreach (var dependencyId in dependencyArray)
        {
            if (dependencyId.IsEmpty)
            {
                throw new ArgumentException(
                    "Dependency IDs must be initialized.",
                    nameof(dependencies));
            }
            if (dependencyId == capabilityId)
            {
                throw new ArgumentException(
                    "A host capability cannot depend on itself.",
                    nameof(dependencies));
            }
            if (!seen.Add(dependencyId))
            {
                throw new ArgumentException(
                    "Dependency IDs must be unique.",
                    nameof(dependencies));
            }
        }

        Array.Sort(dependencyArray);
        return new HostRuntimeCapabilityDescriptor(
            capabilityId,
            payload,
            dependencyArray);
    }
}
