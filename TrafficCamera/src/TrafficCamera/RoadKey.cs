using System.Runtime.InteropServices;

namespace TrafficCamera;

public readonly struct RoadKey : IEquatable<RoadKey>
{
    private readonly int _key;

    private RoadKey(int eKey)
    {
        _key = eKey;
    }

    public static RoadKey Create(ReadOnlySpan<byte> road)
    {
        if (road.Length == 4)
        {
            return new RoadKey(MemoryMarshal.Read<int>(road));
        }

        Span<byte> temp = stackalloc byte[4];
        temp.Clear();
        road.CopyTo(temp);
        return new RoadKey(MemoryMarshal.Read<int>(temp));
    }

    public bool Equals(RoadKey other)
    {
        return _key == other._key;
    }

    public override bool Equals(object? obj)
    {
        return obj is RoadKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return _key;
    }

    public static bool operator ==(RoadKey left, RoadKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(RoadKey left, RoadKey right)
    {
        return !left.Equals(right);
    }
}