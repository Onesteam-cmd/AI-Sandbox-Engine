namespace AI.Sandbox.Engine.Core.ContextRetrieval;

internal static class ContextTypePolicy
{
    internal static void EnsureExactType(
        Type payloadType,
        Type markerType,
        string role)
    {
        ArgumentNullException.ThrowIfNull(payloadType);
        ArgumentNullException.ThrowIfNull(markerType);
        ArgumentException.ThrowIfNullOrWhiteSpace(role);

        if (!markerType.IsAssignableFrom(payloadType))
        {
            throw new ArgumentException(
                $"The {role} type must implement {markerType.FullName}.");
        }

        if (payloadType.IsInterface ||
            payloadType.IsAbstract ||
            payloadType.ContainsGenericParameters ||
            (!payloadType.IsValueType && !payloadType.IsSealed))
        {
            throw new ArgumentException(
                $"The {role} type must be an exact value type or sealed " +
                "reference type.");
        }
    }
}
