namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Defines explicit Host recovery resumption planning and selection outcomes.
/// </summary>
public enum HostRuntimeRecoveryResumptionStatus
{
    /// <summary>A bounded deterministic resumption plan was created.</summary>
    PlanCreated = 0,

    /// <summary>One planned attempt was selected for external resumption.</summary>
    SelectionCreated = 1,

    /// <summary>The optimistic recovery continuation revision did not match.</summary>
    StaleContinuationRevision = 2,

    /// <summary>The planning tick precedes recovery continuation authority.</summary>
    PlanningTickRegressed = 3,

    /// <summary>No pending checkpoint work is eligible for resumption.</summary>
    NoResumableWork = 4,

    /// <summary>The optimistic resumption plan revision did not match.</summary>
    StalePlanRevision = 5,

    /// <summary>The selection tick precedes plan creation.</summary>
    SelectionTickRegressed = 6,

    /// <summary>The requested attempt is not a candidate in the plan.</summary>
    AttemptNotPlanned = 7,
}
