namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit Host recovery resumed-attempt settlement and cycle-completion outcomes.
/// </summary>
public enum HostRuntimeRecoverySettlementStatus
{
    /// <summary>The resumed attempt produced terminal settlement authority.</summary>
    AttemptSettled = 0,

    /// <summary>The exact recovery cycle was closed after terminal settlement.</summary>
    CycleCompleted = 1,

    /// <summary>The optimistic resumed-attempt acknowledgement revision did not match.</summary>
    StaleAcknowledgementRevision = 2,

    /// <summary>The recovery settlement tick preceded acknowledgement.</summary>
    SettlementTickRegressed = 3,

    /// <summary>Existing attempt-settlement contracts rejected the reported completion.</summary>
    AttemptSettlementRejected = 4,

    /// <summary>The optimistic recovery settlement revision did not match.</summary>
    StaleSettlementRevision = 5,

    /// <summary>The recovery-cycle completion tick preceded settlement.</summary>
    CompletionTickRegressed = 6,
}
