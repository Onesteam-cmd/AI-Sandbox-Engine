namespace AI.Sandbox.Engine.Core.Memory;

/// <summary>
/// Provides this memory-model API member.
/// </summary>
public readonly record struct MemorySalience :
    IComparable<MemorySalience>
{
    private const ushort MaximumBasisPoints = 10_000;
    private readonly bool isInitialized;
    private readonly ushort basisPoints;

    private MemorySalience(ushort basisPoints)
    {
        this.basisPoints = basisPoints;
        isInitialized = true;
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static MemorySalience Zero { get; } = new(0);

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static MemorySalience Maximum { get; } =
        new(MaximumBasisPoints);

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public ushort BasisPoints => basisPoints;

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public bool IsEmpty => !isInitialized;

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static MemorySalience FromBasisPoints(ushort value)
    {
        if (value > MaximumBasisPoints)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"Memory salience cannot exceed " +
                $"{MaximumBasisPoints} basis points.");
        }

        return new MemorySalience(value);
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemorySalience Increase(ushort amount)
    {
        EnsureInitialized();

        var increased =
            (uint)basisPoints +
            amount;

        return new MemorySalience(
            (ushort)Math.Min(
                increased,
                (uint)MaximumBasisPoints));
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemorySalience Decrease(ushort amount)
    {
        EnsureInitialized();

        return amount >= basisPoints
            ? Zero
            : new MemorySalience(
                (ushort)(basisPoints - amount));
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public int CompareTo(MemorySalience other)
    {
        EnsureInitialized();
        other.EnsureInitialized();

        return basisPoints.CompareTo(other.basisPoints);
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public override string ToString()
    {
        return IsEmpty
            ? "uninitialized"
            : string.Concat(
                basisPoints.ToString(
                    System.Globalization.CultureInfo.InvariantCulture),
                " bp");
    }

    internal void EnsureInitialized()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(
                "The default memory salience is not initialized.");
        }
    }
}
