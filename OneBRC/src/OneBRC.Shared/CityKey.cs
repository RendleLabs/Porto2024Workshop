using System.Runtime.InteropServices;

namespace OneBRC.Shared;

public struct CityKey : IEquatable<CityKey>
{
    private readonly long _key0;
    private readonly long _key1;
    private readonly long _key2;
    private readonly long _key3;

    private CityKey(long key0, long key1, long key2, long key3)
    {
        _key0 = key0;
        _key1 = key1;
        _key2 = key2;
        _key3 = key3;
    }

    public static CityKey Create(ReadOnlySpan<byte> city)
    {
        if (city.Length < 32)
        {
            Span<byte> temp = stackalloc byte[32];
            temp.Clear();
            city.CopyTo(temp);
            return CreateImpl(temp);
        }
        
        return CreateImpl(city);
    }

    public static CityKey Create(ReadOnlySpan<char> city)
    {
        var bytes = MemoryMarshal.AsBytes(city);

        if (bytes.Length < 32)
        {
            Span<byte> temp = stackalloc byte[32];
            temp.Clear();
            bytes.CopyTo(temp);
            return CreateImpl(temp);
        }
        else
        {
            return CreateImpl(bytes);
        }
    }

    private static CityKey CreateImpl(ReadOnlySpan<byte> bytes)
    {
        var key0 = MemoryMarshal.Read<long>(bytes.Slice(0, 8));
        var key1 = MemoryMarshal.Read<long>(bytes.Slice(8, 8));
        var key2 = MemoryMarshal.Read<long>(bytes.Slice(16, 8));
        var key3 = MemoryMarshal.Read<long>(bytes.Slice(24, 8));
        
        return new CityKey(key0, key1, key2, key3);
    }

    public bool Equals(CityKey other)
    {
        return _key0 == other._key0
            && _key1 == other._key1
            && _key2 == other._key2
            && _key3 == other._key3;
    }

    public override bool Equals(object? obj)
    {
        return obj is CityKey other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_key0, _key1, _key2, _key3);
    }

    public static bool operator ==(CityKey left, CityKey right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(CityKey left, CityKey right)
    {
        return !left.Equals(right);
    }
}