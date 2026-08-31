namespace AI.Sandbox.Engine.Core.WorldState;

/// <summary>
/// Marks an immutable root object that represents one complete authoritative
/// world state.
/// </summary>
/// <remarks>
/// Implementations must be deeply immutable after publication. The core cannot
/// enforce deep immutability of arbitrary object graphs, so state
/// implementations and later component stores are responsible for preserving
/// this contract.
/// </remarks>
public interface IWorldState
{
}
