namespace AI.Sandbox.Engine.Core.Persistence;

/// <summary>
/// Owns an immutable defensive copy of encoded snapshot payload bytes.
/// </summary>
public sealed class SnapshotPayload
{
    private readonly byte[] bytes;

    private SnapshotPayload(byte[] bytes)
    {
        this.bytes = bytes;
    }

    /// <summary>
    /// Gets the shared empty payload.
    /// </summary>
    public static SnapshotPayload Empty { get; } =
        new(Array.Empty<byte>());

    /// <summary>
    /// Gets the number of encoded bytes.
    /// </summary>
    public int Length => bytes.Length;

    /// <summary>
    /// Gets a value indicating whether the payload contains no bytes.
    /// </summary>
    public bool IsEmpty => bytes.Length == 0;

    /// <summary>
    /// Creates an immutable payload by copying the supplied bytes.
    /// </summary>
    /// <param name="data">The bytes to copy.</param>
    /// <returns>An immutable payload.</returns>
    public static SnapshotPayload From(ReadOnlySpan<byte> data)
    {
        return data.IsEmpty
            ? Empty
            : new SnapshotPayload(data.ToArray());
    }

    /// <summary>
    /// Returns a new copy of the encoded bytes.
    /// </summary>
    /// <returns>A caller-owned byte array.</returns>
    public byte[] ToArray()
    {
        return bytes.ToArray();
    }

    /// <summary>
    /// Copies the encoded bytes into a destination span.
    /// </summary>
    /// <param name="destination">
    /// A destination whose length is at least <see cref="Length"/>.
    /// </param>
    public void CopyTo(Span<byte> destination)
    {
        bytes.AsSpan().CopyTo(destination);
    }

    /// <summary>
    /// Compares encoded payload content.
    /// </summary>
    /// <param name="other">The payload to compare.</param>
    /// <returns>
    /// <see langword="true"/> when both payloads contain identical bytes.
    /// </returns>
    public bool ContentEquals(SnapshotPayload? other)
    {
        return other is not null &&
            bytes.AsSpan().SequenceEqual(other.bytes);
    }
}
