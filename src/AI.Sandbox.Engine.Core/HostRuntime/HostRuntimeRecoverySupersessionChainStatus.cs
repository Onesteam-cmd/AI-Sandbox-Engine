namespace AI.Sandbox.Engine.Core.HostRuntime;
/// <summary>
/// Defines explicit Host recovery supersession-chain validation and
/// latest-checkpoint selection outcomes.
/// </summary>
public enum HostRuntimeRecoverySupersessionChainStatus
{
    /// <summary>The ordered supersession chain was validated.</summary>
    ChainValidated = 0,

    /// <summary>The exact latest checkpoint was selected.</summary>
    LatestCheckpointSelected = 1,

    /// <summary>No checkpoint supersessions were supplied.</summary>
    EmptyChain = 2,

    /// <summary>The supplied chain exceeded the bounded item count.</summary>
    TooManySupersessions = 3,

    /// <summary>The optimistic revision list did not match the chain length.</summary>
    SupersessionRevisionCountMismatch = 4,

    /// <summary>An optimistic checkpoint-supersession revision did not match.</summary>
    StaleSupersessionRevision = 5,

    /// <summary>A checkpoint-supersession identity appeared more than once.</summary>
    DuplicateSupersessionId = 6,

    /// <summary>A prior checkpoint identity appeared more than once.</summary>
    DuplicatePriorCheckpointId = 7,

    /// <summary>A successor checkpoint identity appeared more than once.</summary>
    DuplicateSuccessorCheckpointId = 8,

    /// <summary>One chain edge did not begin at the previous successor checkpoint.</summary>
    DisconnectedChain = 9,

    /// <summary>The ordered checkpoint chain closes a cycle.</summary>
    CycleDetected = 10,

    /// <summary>One shared checkpoint identity represented different authority.</summary>
    CheckpointAuthorityMismatch = 11,

    /// <summary>One supersession belongs to another runtime instance.</summary>
    RuntimeMismatch = 12,

    /// <summary>One supersession represents another composition.</summary>
    CompositionMismatch = 13,

    /// <summary>One supersession represents another Host queue.</summary>
    QueueMismatch = 14,

    /// <summary>One supersession uses another monotonic Host clock.</summary>
    ClockMismatch = 15,

    /// <summary>One supersession represents another World identity.</summary>
    WorldMismatch = 16,

    /// <summary>A checkpoint revision regressed across the chain.</summary>
    CheckpointRevisionRegressed = 17,

    /// <summary>A checkpoint capture tick regressed across the chain.</summary>
    CheckpointCaptureTickRegressed = 18,

    /// <summary>A World State version regressed across the chain.</summary>
    WorldStateVersionRegressed = 19,

    /// <summary>A logical simulation tick regressed across the chain.</summary>
    SimulationTickRegressed = 20,

    /// <summary>A supersession tick regressed across the chain.</summary>
    SupersessionTickRegressed = 21,

    /// <summary>The validation tick preceded the latest supersession.</summary>
    ValidationTickRegressed = 22,

    /// <summary>The optimistic supersession-chain revision did not match.</summary>
    StaleChainRevision = 23,

    /// <summary>The selection tick preceded chain validation.</summary>
    SelectionTickRegressed = 24,
}
