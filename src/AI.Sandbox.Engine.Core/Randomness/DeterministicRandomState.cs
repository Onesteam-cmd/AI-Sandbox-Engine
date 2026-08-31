using AI.Sandbox.Engine.Core.Identifiers;

namespace AI.Sandbox.Engine.Core.Randomness;

/// <summary>
/// Represents one immutable, persistable state of one independent deterministic
/// random stream.
/// </summary>
/// <remarks>
/// The implementation is the version-1 SplitMix64 contract. Every draw returns a
/// new state. Callers must store that state in authoritative World State before
/// relying on later values.
/// </remarks>
public readonly record struct DeterministicRandomState
{
    private const ulong Increment = 0x9e3779b97f4a7c15UL;
    private const ulong SeedSalt = 0xd1b54a32d192ed03UL;
    private const ulong FnvOffset = 14695981039346656037UL;
    private const ulong FnvPrime = 1099511628211UL;

    private DeterministicRandomState(
        RandomAlgorithmVersion algorithmVersion,
        Id<RandomStreamIdKind> streamId,
        ulong stateValue,
        ulong drawCount)
    {
        AlgorithmVersion = algorithmVersion;
        StreamId = streamId;
        StateValue = stateValue;
        DrawCount = drawCount;
    }

    /// <summary>
    /// Gets the persisted deterministic algorithm contract version.
    /// </summary>
    public RandomAlgorithmVersion AlgorithmVersion { get; }

    /// <summary>
    /// Gets the stable independent stream identity.
    /// </summary>
    public Id<RandomStreamIdKind> StreamId { get; }

    /// <summary>
    /// Gets the complete internal algorithm state required for persistence.
    /// </summary>
    public ulong StateValue { get; }

    /// <summary>
    /// Gets the number of primitive 64-bit draws already consumed.
    /// </summary>
    public ulong DrawCount { get; }

    /// <summary>
    /// Gets a value indicating whether this is an invalid default state.
    /// </summary>
    public bool IsEmpty =>
        AlgorithmVersion.IsEmpty ||
        StreamId.IsEmpty;

    /// <summary>
    /// Derives one independent stream state from an explicit root seed and
    /// stable stream ID.
    /// </summary>
    /// <param name="seed">The initialized root seed.</param>
    /// <param name="streamId">The non-empty stable stream ID.</param>
    /// <returns>The initial immutable stream state.</returns>
    public static DeterministicRandomState Create(
        RandomSeed seed,
        Id<RandomStreamIdKind> streamId)
    {
        if (seed.IsEmpty)
        {
            throw new ArgumentException(
                "A deterministic random seed cannot be empty.",
                nameof(seed));
        }

        EnsureStreamId(streamId);

        var streamHash = HashCanonicalStreamId(streamId);
        var initialState = Mix(
            seed.Value ^
            streamHash ^
            SeedSalt);

        return new DeterministicRandomState(
            RandomAlgorithmVersion.Current,
            streamId,
            initialState,
            0);
    }

    /// <summary>
    /// Reconstructs one persisted deterministic stream state.
    /// </summary>
    /// <param name="algorithmVersion">
    /// The persisted algorithm contract version.
    /// </param>
    /// <param name="streamId">The non-empty stable stream ID.</param>
    /// <param name="stateValue">The complete persisted internal state.</param>
    /// <param name="drawCount">The persisted primitive draw count.</param>
    /// <returns>The restored immutable stream state.</returns>
    public static DeterministicRandomState Restore(
        RandomAlgorithmVersion algorithmVersion,
        Id<RandomStreamIdKind> streamId,
        ulong stateValue,
        ulong drawCount)
    {
        if (algorithmVersion.IsEmpty)
        {
            throw new ArgumentException(
                "A random algorithm version cannot be empty.",
                nameof(algorithmVersion));
        }

        if (algorithmVersion != RandomAlgorithmVersion.Current)
        {
            throw new NotSupportedException(
                $"Random algorithm version '{algorithmVersion}' is not " +
                "supported by this runtime.");
        }

        EnsureStreamId(streamId);

        return new DeterministicRandomState(
            algorithmVersion,
            streamId,
            stateValue,
            drawCount);
    }

    /// <summary>
    /// Draws one uniformly distributed 64-bit unsigned integer.
    /// </summary>
    /// <returns>The sampled value and exact next state.</returns>
    public RandomDraw<ulong> NextUInt64()
    {
        EnsureInitialized();

        var nextDrawCount = checked(DrawCount + 1);
        var nextStateValue = unchecked(StateValue + Increment);
        var value = Mix(nextStateValue);
        var nextState = new DeterministicRandomState(
            AlgorithmVersion,
            StreamId,
            nextStateValue,
            nextDrawCount);

        return new RandomDraw<ulong>(value, nextState);
    }

    /// <summary>
    /// Draws an unbiased unsigned integer in
    /// <c>[0, exclusiveUpperBound)</c>.
    /// </summary>
    /// <param name="exclusiveUpperBound">The positive exclusive upper bound.</param>
    /// <returns>The sampled value and exact next state.</returns>
    public RandomDraw<ulong> NextUInt64(ulong exclusiveUpperBound)
    {
        EnsureInitialized();

        if (exclusiveUpperBound == 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(exclusiveUpperBound),
                exclusiveUpperBound,
                "The exclusive upper bound must be positive.");
        }

        var threshold =
            unchecked(0UL - exclusiveUpperBound) %
            exclusiveUpperBound;
        var current = this;

        while (true)
        {
            var draw = current.NextUInt64();
            current = draw.NextState;

            if (draw.Value >= threshold)
            {
                return new RandomDraw<ulong>(
                    draw.Value % exclusiveUpperBound,
                    current);
            }
        }
    }

    /// <summary>
    /// Draws an unbiased 32-bit integer in
    /// <c>[minInclusive, maxExclusive)</c>.
    /// </summary>
    /// <param name="minInclusive">The inclusive lower bound.</param>
    /// <param name="maxExclusive">The exclusive upper bound.</param>
    /// <returns>The sampled value and exact next state.</returns>
    public RandomDraw<int> NextInt32(
        int minInclusive,
        int maxExclusive)
    {
        EnsureInitialized();

        if (minInclusive >= maxExclusive)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxExclusive),
                maxExclusive,
                "The exclusive upper bound must exceed the lower bound.");
        }

        var width = (ulong)((long)maxExclusive - minInclusive);
        var draw = NextUInt64(width);
        var value = checked(
            (int)((long)minInclusive + (long)draw.Value));

        return new RandomDraw<int>(value, draw.NextState);
    }

    /// <summary>
    /// Draws a deterministic double in <c>[0, 1)</c> using 53 random bits.
    /// </summary>
    /// <returns>The sampled value and exact next state.</returns>
    public RandomDraw<double> NextDouble()
    {
        EnsureInitialized();

        var draw = NextUInt64();
        var value =
            (draw.Value >> 11) *
            (1.0 / 9007199254740992.0);

        return new RandomDraw<double>(value, draw.NextState);
    }

    /// <summary>
    /// Draws one deterministic Boolean value.
    /// </summary>
    /// <returns>The sampled value and exact next state.</returns>
    public RandomDraw<bool> NextBoolean()
    {
        EnsureInitialized();

        var draw = NextUInt64();

        return new RandomDraw<bool>(
            (draw.Value & 1UL) != 0,
            draw.NextState);
    }

    private static void EnsureStreamId(
        Id<RandomStreamIdKind> streamId)
    {
        if (streamId.IsEmpty)
        {
            throw new ArgumentException(
                "A deterministic random stream ID cannot be empty.",
                nameof(streamId));
        }
    }

    private void EnsureInitialized()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(
                "The default deterministic random state is not initialized.");
        }

        if (AlgorithmVersion != RandomAlgorithmVersion.Current)
        {
            throw new NotSupportedException(
                $"Random algorithm version '{AlgorithmVersion}' is not " +
                "supported by this runtime.");
        }
    }

    private static ulong HashCanonicalStreamId(
        Id<RandomStreamIdKind> streamId)
    {
        var hash = FnvOffset;

        foreach (var character in streamId.ToString())
        {
            hash ^= character;
            hash = unchecked(hash * FnvPrime);
        }

        return hash;
    }

    private static ulong Mix(ulong value)
    {
        var mixed = value;
        mixed = unchecked(
            (mixed ^ (mixed >> 30)) *
            0xbf58476d1ce4e5b9UL);
        mixed = unchecked(
            (mixed ^ (mixed >> 27)) *
            0x94d049bb133111ebUL);

        return mixed ^ (mixed >> 31);
    }
}
