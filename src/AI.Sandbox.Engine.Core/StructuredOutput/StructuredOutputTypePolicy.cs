namespace AI.Sandbox.Engine.Core.StructuredOutput;

internal static class StructuredOutputTypePolicy
{
    public static void EnsureExactType(
        Type payloadType,
        Type contractType,
        string role)
    {
        ArgumentNullException.ThrowIfNull(payloadType);
        ArgumentNullException.ThrowIfNull(contractType);
        ArgumentException.ThrowIfNullOrWhiteSpace(role);

        if (!contractType.IsAssignableFrom(payloadType))
        {
            throw new ArgumentException(
                $"The {role} type must implement {contractType.FullName}.");
        }

        if (payloadType == contractType ||
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
