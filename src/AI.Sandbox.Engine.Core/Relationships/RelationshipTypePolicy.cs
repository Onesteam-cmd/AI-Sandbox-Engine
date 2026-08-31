namespace AI.Sandbox.Engine.Core.Relationships;

internal static class RelationshipTypePolicy
{
    public static void EnsureConcrete<TState>(string parameterName)
        where TState : notnull, IRelationshipState
    {
        var stateType = typeof(TState);

        if (stateType.ContainsGenericParameters ||
            stateType.IsInterface ||
            stateType.IsAbstract ||
            (stateType.IsClass && !stateType.IsSealed))
        {
            throw new ArgumentException(
                $"Relationship state type '{stateType}' must be a concrete " +
                "value type or a sealed reference type.",
                parameterName);
        }
    }

    public static void EnsureValue<TState>(
        TState state,
        string parameterName)
        where TState : notnull, IRelationshipState
    {
        ArgumentNullException.ThrowIfNull(
            state,
            parameterName);
    }
}
