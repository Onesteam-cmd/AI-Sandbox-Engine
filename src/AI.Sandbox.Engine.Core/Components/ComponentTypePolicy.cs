namespace AI.Sandbox.Engine.Core.Components;

internal static class ComponentTypePolicy
{
    public static void EnsureConcrete<TComponent>()
        where TComponent : notnull, IComponent
    {
        var componentType = typeof(TComponent);

        if (componentType.ContainsGenericParameters ||
            componentType.IsInterface ||
            componentType.IsAbstract ||
            (componentType.IsClass && !componentType.IsSealed))
        {
            throw new ArgumentException(
                $"Component type '{componentType}' must be a concrete value " +
                "type or a sealed reference type.",
                "TComponent");
        }
    }

    public static void EnsureValue<TComponent>(TComponent component)
        where TComponent : notnull, IComponent
    {
        ArgumentNullException.ThrowIfNull(component);
    }
}
