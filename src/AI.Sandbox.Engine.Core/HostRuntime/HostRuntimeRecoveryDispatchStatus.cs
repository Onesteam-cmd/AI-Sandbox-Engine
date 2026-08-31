namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit Host recovery dispatch reconstruction and resumed-attempt
/// acknowledgement outcomes.
/// </summary>
public enum HostRuntimeRecoveryDispatchStatus
{
    /// <summary>New recovery dispatch authority was reconstructed.</summary>
    DispatchReconstructed = 0,

    /// <summary>The reconstructed dispatch was acknowledged as a new attempt.</summary>
    AttemptAcknowledged = 1,

    /// <summary>The optimistic lease-reacquisition revision did not match.</summary>
    StaleReacquisitionRevision = 2,

    /// <summary>The reconstruction tick preceded lease reacquisition.</summary>
    ReconstructionTickRegressed = 3,

    /// <summary>The supplied recovery queue did not match reacquired authority.</summary>
    QueueMismatch = 4,

    /// <summary>The prior dispatch-selection ID was reused.</summary>
    PriorSelectionIdReused = 5,

    /// <summary>The prior dispatch ID was reused.</summary>
    PriorDispatchIdReused = 6,

    /// <summary>The resumed dispatch attempt number was not the next attempt.</summary>
    AttemptNumberMismatch = 7,

    /// <summary>Existing dispatch-selection contracts rejected reconstruction.</summary>
    DispatchSelectionRejected = 8,

    /// <summary>The optimistic reconstruction revision did not match.</summary>
    StaleReconstructionRevision = 9,

    /// <summary>The acknowledgement tick preceded reconstruction.</summary>
    AcknowledgementTickRegressed = 10,

    /// <summary>The prior checkpoint attempt ID was reused.</summary>
    PriorAttemptIdReused = 11,

    /// <summary>
    /// Existing dispatch-acknowledgement contracts rejected the resumed attempt.
    /// </summary>
    DispatchAcknowledgementRejected = 12,
}
