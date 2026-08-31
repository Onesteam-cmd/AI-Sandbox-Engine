namespace AI.Sandbox.Engine.Core.Commands;

/// <summary>
/// Marks an immutable request to change authoritative World State.
/// </summary>
/// <remarks>
/// A command is an intention or request, not a completed fact. Commands may be
/// rejected and must never be confused with immutable engine events.
/// </remarks>
public interface IEngineCommand
{
}
