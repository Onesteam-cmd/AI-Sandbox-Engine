namespace AI.Sandbox.Engine.Core.Events;

/// <summary>
/// Marks an immutable fact notification produced by an engine subsystem.
/// </summary>
/// <remarks>
/// Events describe something that has already happened. They are not commands,
/// do not mutate World State directly, and should remain immutable after
/// publication.
/// </remarks>
public interface IEngineEvent
{
}
