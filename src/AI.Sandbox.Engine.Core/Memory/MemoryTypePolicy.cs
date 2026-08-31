namespace AI.Sandbox.Engine.Core.Memory;

internal static class MemoryTypePolicy
{
    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static void EnsureConcrete<TContent>(string parameterName)
        where TContent : notnull, IMemoryContent
    {
        var contentType = typeof(TContent);

        if (contentType.ContainsGenericParameters ||
            contentType.IsInterface ||
            contentType.IsAbstract ||
            (contentType.IsClass && !contentType.IsSealed))
        {
            throw new ArgumentException(
                $"Memory content type '{contentType}' must be a concrete value " +
                "type or a sealed reference type.",
                parameterName);
        }
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static void EnsureValue<TContent>(
        TContent content,
        string parameterName)
        where TContent : notnull, IMemoryContent
    {
        ArgumentNullException.ThrowIfNull(
            content,
            parameterName);
    }
}
