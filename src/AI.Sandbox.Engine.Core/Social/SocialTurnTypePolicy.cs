namespace AI.Sandbox.Engine.Core.Social;

internal static class SocialTurnTypePolicy
{
    public static void EnsureExactType(
        Type payloadType,
        Type markerType,
        string description)
    {
        ArgumentNullException.ThrowIfNull(payloadType);
        ArgumentNullException.ThrowIfNull(markerType);

        if (!markerType.IsAssignableFrom(payloadType))
        {
            throw new ArgumentException(
                $"The {description} type must implement {markerType.Name}.");
        }

        if (payloadType.IsInterface ||
            payloadType.IsAbstract ||
            payloadType.ContainsGenericParameters)
        {
            throw new ArgumentException(
                $"The {description} type must be exact and constructible.");
        }

        if (!payloadType.IsValueType && !payloadType.IsSealed)
        {
            throw new ArgumentException(
                $"The {description} reference type must be sealed.");
        }
    }
}
