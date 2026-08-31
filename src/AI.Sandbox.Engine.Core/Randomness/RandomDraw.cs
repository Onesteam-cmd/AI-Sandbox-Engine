namespace AI.Sandbox.Engine.Core.Randomness;

/// <summary>
/// Couples one deterministic sampled value with the exact next immutable random
/// state.
/// </summary>
/// <typeparam name="T">The sampled value type.</typeparam>
public readonly record struct RandomDraw<T>
{
    internal RandomDraw(
        T value,
        DeterministicRandomState nextState)
    {
        Value = value;
        NextState = nextState;
    }

    /// <summary>
    /// Gets the sampled value.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Gets the state that must replace the previous state before another draw.
    /// </summary>
    public DeterministicRandomState NextState { get; }
}
