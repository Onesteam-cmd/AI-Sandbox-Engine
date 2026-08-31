namespace AI.Sandbox.Engine.Core.Components;

/// <summary>
/// Marks immutable data that can be attached to an active entity.
/// </summary>
/// <remarks>
/// Components contain state only. They do not own behavior, dispatch events,
/// access services, or mutate World State. Component implementations must be
/// immutable after publication. Concrete value types and sealed reference types
/// are supported.
/// </remarks>
public interface IComponent
{
}
