namespace AI.Sandbox.Engine.Core.Memory;

/// <summary>
/// Provides this memory-model API member.
/// </summary>
public readonly record struct MemoryStrength :
    IComparable<MemoryStrength>
{
    private const ushort MaximumBasisPoints = 10_000;
    private readonly bool isInitialized;
    private readonly ushort basisPoints;

    private MemoryStrength(ushort basisPoints)
    {
        this.basisPoints = basisPoints;
        isInitialized = true;
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static MemoryStrength Zero { get; } = new(0);

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static MemoryStrength Maximum { get; } =
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
    public bool IsZero =>
        isInitialized &&
        basisPoints == 0;

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public static MemoryStrength FromBasisPoints(ushort value)
    {
        if (value > MaximumBasisPoints)
        {
            throw new ArgumentOutOfRangeException(
                nameof(value),
                value,
                $"Memory strength cannot exceed " +
                $"{MaximumBasisPoints} basis points.");
        }

        return new MemoryStrength(value);
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemoryStrength Increase(ushort amount)
    {
        EnsureInitialized();

        var increased =
            (uint)basisPoints +
            amount;

        return new MemoryStrength(
            (ushort)Math.Min(
                increased,
                (uint)MaximumBasisPoints));
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public MemoryStrength Decrease(ushort amount)
    {
        EnsureInitialized();

        return amount >= basisPoints
            ? Zero
            : new MemoryStrength(
                (ushort)(basisPoints - amount));
    }

    /// <summary>
    /// Provides this memory-model API member.
    /// </summary>
    public int CompareTo(MemoryStrength other)
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

    internal void EnsureUsableForRetainedMemory()
    {
        EnsureInitialized();

        if (IsZero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(MemoryStrength),
                this,
                "A retained memory requires strength above zero.");
        }
    }

    internal void EnsureInitialized()
    {
        if (IsEmpty)
        {
            throw new InvalidOperationException(
                "The default memory strength is not initialized.");
        }
    }
}
