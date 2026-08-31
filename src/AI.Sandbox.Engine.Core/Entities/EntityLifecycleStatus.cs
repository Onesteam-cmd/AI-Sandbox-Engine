namespace AI.Sandbox.Engine.Core.Entities;

/// <summary>
/// Identifies the lifecycle state of an entity identifier within one world.
/// </summary>
public enum EntityLifecycleStatus
{
    /// <summary>
    /// The identifier has never been registered in this world.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// The entity currently exists and may participate in simulation.
    /// </summary>
    Active = 1,

    /// <summary>
    /// The entity no longer exists, but its identifier remains reserved.
    /// </summary>
    Destroyed = 2,
}
