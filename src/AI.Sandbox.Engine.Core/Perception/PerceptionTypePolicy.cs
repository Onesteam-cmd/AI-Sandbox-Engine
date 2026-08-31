namespace AI.Sandbox.Engine.Core.Perception;

internal static class PerceptionTypePolicy
{
    public static void EnsureConcrete<TValue>(string parameterName)
        where TValue : notnull
    {
        var valueType = typeof(TValue);

        if (valueType.ContainsGenericParameters ||
            valueType.IsInterface ||
            valueType.IsAbstract ||
            (valueType.IsClass && !valueType.IsSealed))
        {
            throw new ArgumentException(
                $"Perception type '{valueType}' must be a concrete value type " +
                "or a sealed reference type.",
                parameterName);
        }
    }

    public static void EnsureValue<TValue>(
        TValue value,
        string parameterName)
        where TValue : notnull
    {
        ArgumentNullException.ThrowIfNull(
            value,
            parameterName);
    }
}
