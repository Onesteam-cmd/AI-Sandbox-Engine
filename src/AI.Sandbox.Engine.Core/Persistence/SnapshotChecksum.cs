namespace AI.Sandbox.Engine.Core.Persistence;

/// <summary>
/// Represents a canonical lowercase SHA-256 checksum of snapshot payload bytes.
/// </summary>
public readonly record struct SnapshotChecksum
{
    private const int HexLength = 64;
    private readonly string? value;

    private SnapshotChecksum(string value)
    {
        this.value = value;
    }

    /// <summary>
    /// Gets the canonical lowercase hexadecimal checksum.
    /// </summary>
    public string Value => value ?? string.Empty;

    /// <summary>
    /// Gets a value indicating whether this is the invalid default value.
    /// </summary>
    public bool IsEmpty => value is null;

    /// <summary>
    /// Computes the SHA-256 checksum of an immutable payload.
    /// </summary>
    /// <param name="payload">The payload to hash.</param>
    /// <returns>The canonical checksum.</returns>
    public static SnapshotChecksum Compute(SnapshotPayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        var hash = System.Security.Cryptography.SHA256.HashData(
            payload.ToArray());
        var canonical = Convert
            .ToHexString(hash)
            .ToLowerInvariant();

        return new SnapshotChecksum(canonical);
    }

    /// <summary>
    /// Parses a canonical hexadecimal checksum.
    /// </summary>
    /// <param name="value">The 64-character hexadecimal text.</param>
    /// <returns>The parsed checksum.</returns>
    public static SnapshotChecksum Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!TryParse(value, out var result))
        {
            throw new FormatException(
                "A snapshot checksum must contain exactly 64 hexadecimal " +
                "characters.");
        }

        return result;
    }

    /// <summary>
    /// Attempts to parse hexadecimal checksum text.
    /// </summary>
    /// <param name="value">The checksum text.</param>
    /// <param name="result">
    /// The parsed checksum, or the default value when invalid.
    /// </param>
    /// <returns><see langword="true"/> when parsing succeeds.</returns>
    public static bool TryParse(
        string? value,
        out SnapshotChecksum result)
    {
        if (value is null || value.Length != HexLength)
        {
            result = default;
            return false;
        }

        foreach (var character in value)
        {
            var isDigit = character is >= '0' and <= '9';
            var isLowerHex = character is >= 'a' and <= 'f';
            var isUpperHex = character is >= 'A' and <= 'F';

            if (!isDigit && !isLowerHex && !isUpperHex)
            {
                result = default;
                return false;
            }
        }

        result = new SnapshotChecksum(value.ToLowerInvariant());
        return true;
    }

    /// <summary>
    /// Verifies this checksum against payload bytes.
    /// </summary>
    /// <param name="payload">The payload to verify.</param>
    /// <returns>
    /// <see langword="true"/> when the payload has this checksum.
    /// </returns>
    public bool Matches(SnapshotPayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);

        return !IsEmpty && Compute(payload) == this;
    }

    /// <summary>
    /// Returns the canonical lowercase hexadecimal checksum.
    /// </summary>
    /// <returns>The checksum text.</returns>
    public override string ToString()
    {
        return Value;
    }
}
