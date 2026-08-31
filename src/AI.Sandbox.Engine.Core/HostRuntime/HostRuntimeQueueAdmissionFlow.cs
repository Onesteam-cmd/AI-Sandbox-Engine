namespace AI.Sandbox.Engine.Core.HostRuntime;

/// <summary>
/// Produces pure immutable Host queue-admission and priority authority.
/// </summary>
public static class HostRuntimeQueueAdmissionFlow
{
    /// <summary>Evaluates one optimistic bounded queue admission.</summary>
    public static HostRuntimeQueueAdmissionResult<TRequest>
        Decide<TRequest>(
            global::AI.Sandbox.Engine.Core.Identifiers.Id<
                HostRuntimeAdmissionIdKind> admissionId,
            HostRuntimeQueueSnapshot snapshot,
            long expectedQueueRevision,
            HostRuntimeRequestEnvelope<TRequest> request,
            HostRuntimePriority priority)
        where TRequest : IHostRuntimeRequest
    {
        if (admissionId.IsEmpty)
        {
            throw new ArgumentException(
                "The admission ID must be initialized.",
                nameof(admissionId));
        }
        ArgumentNullException.ThrowIfNull(snapshot);
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(priority);
        if (expectedQueueRevision < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedQueueRevision));
        }

        if (snapshot.Revision != expectedQueueRevision)
        {
            return Unchanged<TRequest>(
                HostRuntimeQueueAdmissionStatus.StaleQueueRevision,
                snapshot);
        }
        if (request.State != HostRuntimeRequestState.Pending)
        {
            return Unchanged<TRequest>(
                HostRuntimeQueueAdmissionStatus.InvalidRequestState,
                snapshot);
        }
        if (snapshot.IsFull)
        {
            return Unchanged<TRequest>(
                HostRuntimeQueueAdmissionStatus.QueueFull,
                snapshot);
        }

        var nextSnapshot = new HostRuntimeQueueSnapshot(
            snapshot.QueueId,
            snapshot.Capacity,
            checked(snapshot.QueuedCount + 1),
            checked(snapshot.Revision + 1));
        var admission =
            new HostRuntimeQueueAdmission<TRequest>(
                admissionId,
                nextSnapshot,
                request,
                priority,
                snapshot.Revision);

        return new HostRuntimeQueueAdmissionResult<TRequest>(
            HostRuntimeQueueAdmissionStatus.Admitted,
            nextSnapshot,
            admission);
    }

    private static HostRuntimeQueueAdmissionResult<TRequest>
        Unchanged<TRequest>(
            HostRuntimeQueueAdmissionStatus status,
            HostRuntimeQueueSnapshot snapshot)
        where TRequest : IHostRuntimeRequest =>
        new(status, snapshot, admission: null);
}
