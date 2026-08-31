namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines explicit Host queue-admission outcomes.</summary>
public enum HostRuntimeQueueAdmissionStatus
{
    /// <summary>The request received immutable admission authority.</summary>
    Admitted = 0,

    /// <summary>The optimistic expected queue revision did not match.</summary>
    StaleQueueRevision = 1,

    /// <summary>The request is not pending and cannot be admitted.</summary>
    InvalidRequestState = 2,

    /// <summary>The represented queue has reached its bounded capacity.</summary>
    QueueFull = 3,
}
