namespace AI.Sandbox.Engine.Core.Events;

/// <summary>
/// Dispatches validated event envelopes to handlers registered for the exact
/// payload type.
/// </summary>
/// <remarks>
/// Handlers for one dispatch call execute sequentially in registration order.
/// The dispatcher does not create threads, start tasks, retain events, retry
/// failures, or apply changes to World State. A handler exception stops the
/// current chain and is propagated to the caller.
/// </remarks>
public sealed class EventDispatcher
{
    private readonly IReadOnlyDictionary<Type, object> handlersByEventType;

    internal EventDispatcher(Dictionary<Type, object> handlersByEventType)
    {
        this.handlersByEventType =
            new System.Collections.ObjectModel.ReadOnlyDictionary<Type, object>(
                handlersByEventType);
    }

    /// <summary>
    /// Dispatches an event to handlers registered for exactly
    /// <typeparamref name="TEvent"/>.
    /// </summary>
    /// <typeparam name="TEvent">The exact event payload type.</typeparam>
    /// <param name="envelope">The validated event envelope.</param>
    /// <param name="cancellationToken">
    /// A token used to cancel before or between handlers.
    /// </param>
    /// <returns>A value task that represents completion of all matching handlers.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="envelope"/> is the invalid default value.
    /// </exception>
    /// <exception cref="OperationCanceledException">
    /// Thrown when <paramref name="cancellationToken"/> is canceled.
    /// </exception>
    public async ValueTask DispatchAsync<TEvent>(
        EventEnvelope<TEvent> envelope,
        CancellationToken cancellationToken = default)
        where TEvent : IEngineEvent
    {
        if (!envelope.IsValid)
        {
            throw new ArgumentException(
                "Only validated event envelopes can be dispatched.",
                nameof(envelope));
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (!handlersByEventType.TryGetValue(typeof(TEvent), out var registered))
        {
            return;
        }

        var handlers = (IReadOnlyList<IEventHandler<TEvent>>)registered;

        foreach (var handler in handlers)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await handler
                .HandleAsync(envelope, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
