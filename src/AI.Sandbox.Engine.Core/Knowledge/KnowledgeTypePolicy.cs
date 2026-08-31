namespace AI.Sandbox.Engine.Core.Knowledge;

internal static class KnowledgeTypePolicy
{
    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public static void EnsureConcrete<TClaim>(string parameterName)
        where TClaim : notnull, IKnowledgeClaim
    {
        var claimType = typeof(TClaim);

        if (claimType.ContainsGenericParameters ||
            claimType.IsInterface ||
            claimType.IsAbstract ||
            (claimType.IsClass && !claimType.IsSealed))
        {
            throw new ArgumentException(
                $"Knowledge claim type '{claimType}' must be a concrete value " +
                "type or a sealed reference type.",
                parameterName);
        }
    }

    /// <summary>
    /// Provides this knowledge-model API member.
    /// </summary>
    public static void EnsureValue<TClaim>(
        TClaim claim,
        string parameterName)
        where TClaim : notnull, IKnowledgeClaim
    {
        ArgumentNullException.ThrowIfNull(
            claim,
            parameterName);
    }
}
