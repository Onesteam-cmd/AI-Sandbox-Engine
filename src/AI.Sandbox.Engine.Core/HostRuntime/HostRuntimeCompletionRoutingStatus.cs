namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>Defines explicit Host completion-routing outcomes.</summary>
public enum HostRuntimeCompletionRoutingStatus
{
    /// <summary>The completion was matched and produced terminal request authority.</summary>
    Routed = 0,

    /// <summary>The optimistic request revision did not match.</summary>
    StaleRevision = 1,

    /// <summary>The current request state is not routable.</summary>
    InvalidRequestState = 2,

    /// <summary>The dispatch does not belong to the current request authority.</summary>
    DispatchMismatch = 3,

    /// <summary>The completion identity does not match the dispatch.</summary>
    CompletionMismatch = 4,
}
