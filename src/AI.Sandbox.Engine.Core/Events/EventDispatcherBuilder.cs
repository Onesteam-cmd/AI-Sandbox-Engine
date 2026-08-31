namespace AI.Sandbox.Engine.Core.Events;

/// <summary>
/// Collects exact-type event handlers before creating an immutable dispatcher
/// registration table.
/// </summary>
/// <remarks>
/// The builder is intentionally single-use. Registration order is preserved and
/// becomes the deterministic invocation order for each event type.
/// </remarks>
public sealed class EventDispatcherBuilder
{
    private readonly Dictionary<Type, object> handlersByEventType = [];
    private bool isBuilt;

    /// <summary>
    /// Registers an event handler.
    /// </summary>
    /// <typeparam name="TEvent">The exact event payload type.</typeparam>
    /// <param name="handler">The handler instance to register.</param>
    /// <returns>This builder for fluent registration.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="handler"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the builder has already produced a dispatcher.
    /// </exception>
    public EventDispatcherBuilder Add<TEvent>(IEventHandler<TEvent> handler)
        where TEvent : IEngineEvent
    {
        ThrowIfBuilt();
        ArgumentNullException.ThrowIfNull(handler);

        if (!handlersByEventType.TryGetValue(typeof(TEvent), out var existing))
        {
            existing = new List<IEventHandler<TEvent>>();
            handlersByEventType.Add(typeof(TEvent), existing);
        }

        var typedHandlers = (List<IEventHandler<TEvent>>)existing;
        typedHandlers.Add(handler);

        return this;
    }

    /// <summary>
    /// Creates a dispatcher from the current registration table and permanently
    /// freezes this builder.
    /// </summary>
    /// <returns>An event dispatcher.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the builder has already produced a dispatcher.
    /// </exception>
    public EventDispatcher Build()
    {
        ThrowIfBuilt();
        isBuilt = true;

        var snapshot = new Dictionary<Type, object>(handlersByEventType);
        return new EventDispatcher(snapshot);
    }

    private void ThrowIfBuilt()
    {
        if (isBuilt)
        {
            throw new InvalidOperationException(
                "An event dispatcher builder cannot be reused after Build.");
        }
    }
}
