namespace AI.Sandbox.Engine.Core.Commands;

internal static class CommandTypePolicy
{
    public static void EnsureConcrete<TCommand>()
        where TCommand : notnull, IEngineCommand
    {
        var commandType = typeof(TCommand);

        if (commandType.ContainsGenericParameters ||
            commandType.IsInterface ||
            commandType.IsAbstract ||
            (commandType.IsClass && !commandType.IsSealed))
        {
            throw new ArgumentException(
                $"Command type '{commandType}' must be a concrete value type " +
                "or a sealed reference type.",
                "TCommand");
        }
    }

    public static void EnsureValue<TCommand>(TCommand command)
        where TCommand : notnull, IEngineCommand
    {
        ArgumentNullException.ThrowIfNull(command);
    }
}
