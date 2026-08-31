namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit Host recovery checkpoint-supersession and cycle-summary outcomes.
/// </summary>
public enum HostRuntimeRecoveryCheckpointSupersessionStatus
{
    /// <summary>The completed cycle was linked to one successor checkpoint.</summary>
    CheckpointSuperseded = 0,

    /// <summary>A compact immutable completed-cycle summary was created.</summary>
    SummaryCreated = 1,

    /// <summary>The optimistic recovery-cycle completion revision did not match.</summary>
    StaleCycleCompletionRevision = 2,

    /// <summary>The optimistic successor-checkpoint revision did not match.</summary>
    StaleSuccessorCheckpointRevision = 3,

    /// <summary>The successor reused the superseded checkpoint identity.</summary>
    CheckpointIdReused = 4,

    /// <summary>The successor checkpoint belongs to another runtime instance.</summary>
    RuntimeMismatch = 5,

    /// <summary>The successor checkpoint represents another composition.</summary>
    CompositionMismatch = 6,

    /// <summary>The successor checkpoint represents another Host queue.</summary>
    QueueMismatch = 7,

    /// <summary>The successor checkpoint uses another monotonic Host clock.</summary>
    ClockMismatch = 8,

    /// <summary>The successor checkpoint represents another World identity.</summary>
    WorldMismatch = 9,

    /// <summary>The successor checkpoint was captured before cycle completion.</summary>
    SuccessorCheckpointTickRegressed = 10,

    /// <summary>The successor checkpoint revision did not advance.</summary>
    SuccessorCheckpointRevisionNotAdvanced = 11,

    /// <summary>The successor World State version regressed.</summary>
    WorldStateVersionRegressed = 12,

    /// <summary>The successor logical simulation tick regressed.</summary>
    SimulationTickRegressed = 13,

    /// <summary>The supersession tick preceded successor checkpoint capture.</summary>
    SupersessionTickRegressed = 14,

    /// <summary>The optimistic checkpoint-supersession revision did not match.</summary>
    StaleSupersessionRevision = 15,

    /// <summary>The summary tick preceded checkpoint supersession.</summary>
    SummaryTickRegressed = 16,
}
