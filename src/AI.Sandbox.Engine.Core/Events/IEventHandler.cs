namespace AI.Sandbox.Engine.Core.Events;

/// <summary>
/// Handles one exact engine event payload type.
/// </summary>
/// <typeparam name="TEvent">The event payload type handled by this instance.</typeparam>
public interface IEventHandler<TEvent>
    where TEvent : IEngineEvent
{
    /// <summary>
    /// Handles one validated event envelope.
    /// </summary>
    /// <param name="envelope">The event occurrence to handle.</param>
    /// <param name="cancellationToken">
    /// A token used to cancel the current dispatch operation.
    /// </param>
    /// <returns>A value task that represents handler completion.</returns>
    public ValueTask HandleAsync(
        EventEnvelope<TEvent> envelope,
        CancellationToken cancellationToken);
}
