namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Validates and deterministically orders an explicit host capability graph.
/// </summary>
public static class HostRuntimeCompositionBuilder
{
    private const int MaximumCapabilityCount = 128;

    /// <summary>
    /// Validates and deterministically orders a bounded capability graph.
    /// </summary>
    /// <param name="compositionId">Externally assigned non-empty composition ID.</param>
    /// <param name="capabilities">Capability descriptors to validate and order.</param>
    /// <returns>An explicit immutable composition result.</returns>
    public static HostRuntimeCompositionResult Compose(
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeCompositionIdKind> compositionId,
        IEnumerable<HostRuntimeCapabilityDescriptor> capabilities)
    {
        if (compositionId.IsEmpty)
        {
            throw new ArgumentException(
                "The composition ID must be initialized.",
                nameof(compositionId));
        }

        ArgumentNullException.ThrowIfNull(capabilities);
        var input = capabilities.ToArray();

        if (input.Length == 0)
        {
            return Result(HostRuntimeCompositionStatus.Empty);
        }
        if (input.Length > MaximumCapabilityCount)
        {
            return Result(HostRuntimeCompositionStatus.TooManyCapabilities);
        }
        if (input.Any(static capability => capability is null))
        {
            throw new ArgumentException(
                "Capability descriptors cannot contain null.",
                nameof(capabilities));
        }

        var byId = new Dictionary<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeCapabilityIdKind>,
            HostRuntimeCapabilityDescriptor>();
        foreach (var capability in input)
        {
            if (!byId.TryAdd(capability.CapabilityId, capability))
            {
                return Result(
                    HostRuntimeCompositionStatus.DuplicateCapability,
                    capability.CapabilityId);
            }
        }

        foreach (var capability in input)
        {
            foreach (var dependencyId in capability.Dependencies)
            {
                if (!byId.ContainsKey(dependencyId))
                {
                    return Result(
                        HostRuntimeCompositionStatus.MissingDependency,
                        capability.CapabilityId);
                }
            }
        }

        var indegree = byId.Keys.ToDictionary(
            static capabilityId => capabilityId,
            static _ => 0);
        var dependents = byId.Keys.ToDictionary(
            static capabilityId => capabilityId,
            static _ => new List<
                global::AI.Sandbox.Engine.Core.Identifiers.Id<
                    HostRuntimeCapabilityIdKind>>());

        foreach (var capability in input)
        {
            indegree[capability.CapabilityId] = capability.Dependencies.Count;
            foreach (var dependencyId in capability.Dependencies)
            {
                dependents[dependencyId].Add(capability.CapabilityId);
            }
        }

        var ready = new SortedSet<
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeCapabilityIdKind>>(
            indegree
                .Where(static pair => pair.Value == 0)
                .Select(static pair => pair.Key));
        var ordered = new List<HostRuntimeCapabilityDescriptor>(input.Length);

        while (ready.Count > 0)
        {
            var capabilityId = ready.Min;
            ready.Remove(capabilityId);
            ordered.Add(byId[capabilityId]);

            foreach (var dependentId in dependents[capabilityId]
                         .OrderBy(static id => id))
            {
                indegree[dependentId] = checked(indegree[dependentId] - 1);
                if (indegree[dependentId] == 0)
                {
                    ready.Add(dependentId);
                }
            }
        }

        if (ordered.Count != input.Length)
        {
            var cycleId = indegree
                .Where(static pair => pair.Value > 0)
                .Select(static pair => pair.Key)
                .OrderBy(static id => id)
                .First();
            return Result(
                HostRuntimeCompositionStatus.CycleDetected,
                cycleId);
        }

        return new HostRuntimeCompositionResult(
            HostRuntimeCompositionStatus.Composed,
            new HostRuntimeComposition(compositionId, ordered.ToArray()),
            default);
    }

    private static HostRuntimeCompositionResult Result(
        HostRuntimeCompositionStatus status,
        global::AI.Sandbox.Engine.Core.Identifiers.Id<
            HostRuntimeCapabilityIdKind> relatedCapabilityId = default) =>
        new(status, null, relatedCapabilityId);
}
