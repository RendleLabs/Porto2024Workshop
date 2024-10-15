using System.Numerics;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

public class VectorKeys
{
    private static readonly ReadOnlyMemory<byte> City = "Ankh-Morpork"u8.ToArray();
    
    private static readonly Key CityKey = Key.Create(City.Span);
    private static readonly VectorKey CityVectorKey = VectorKey.Create(City.Span);

    private static readonly Dictionary<Key, int> KeyDict = new()
    {
        [Key.Create(City.Span)] = 42,
    };

    private static readonly Dictionary<VectorKey, int> VectorKeyDict = new()
    {
        [VectorKey.Create(City.Span)] = 42,
    };

    [Benchmark(Baseline = true)]
    public bool NonAccelerated()
    {
        return Key.Create(City.Span).Equals(CityKey);
    }

    [Benchmark]
    public bool Vector()
    {
        return VectorKey.Create(City.Span).Equals(CityVectorKey);
    }

    public readonly struct Key : IEquatable<Key>
    {
        private readonly long _k0;
        private readonly long _k1;
        private readonly long _k2;
        private readonly long _k3;

        public Key(long k0, long k1, long k2, long k3)
        {
            _k0 = k0;
            _k1 = k1;
            _k2 = k2;
            _k3 = k3;
        }
        

        public static Key Create(ReadOnlySpan<byte> span)
        {
            Span<byte> s = stackalloc byte[32];
            s.Clear();
            span.CopyTo(s);
            var k0 = MemoryMarshal.Read<long>(s);
            var k1 = MemoryMarshal.Read<long>(s.Slice(8));
            var k2 = MemoryMarshal.Read<long>(s.Slice(16));
            var k3 = MemoryMarshal.Read<long>(s.Slice(24));
            return new Key(k0, k1, k2, k3);
        }

        public bool Equals(Key other)
        {
            return _k0 == other._k0 && _k1 == other._k1 && _k2 == other._k2 && _k3 == other._k3;
        }

        public override bool Equals(object? obj)
        {
            return obj is Key other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_k0, _k1, _k2, _k3);
        }

        public static bool operator ==(Key left, Key right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Key left, Key right)
        {
            return !left.Equals(right);
        }
    }

    public readonly struct VectorKey : IEquatable<VectorKey>
    {
        private readonly Vector256<long> _key;

        private VectorKey(long k0, long k1, long k2, long k3)
        {
            _key = Vector256.Create(k0, k1, k2, k3);
        }

        public static VectorKey Create(ReadOnlySpan<byte> span)
        {
            Span<byte> s = stackalloc byte[32];
            s.Clear();
            span.CopyTo(s);
            var k0 = MemoryMarshal.Read<long>(s);
            var k1 = MemoryMarshal.Read<long>(s.Slice(8));
            var k2 = MemoryMarshal.Read<long>(s.Slice(16));
            var k3 = MemoryMarshal.Read<long>(s.Slice(24));
            return new VectorKey(k0, k1, k2, k3);
        }

        public bool Equals(VectorKey other)
        {
            return Vector256.EqualsAll(_key, other._key);
        }

        public override bool Equals(object? obj)
        {
            return obj is VectorKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }

        public static bool operator ==(VectorKey left, VectorKey right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(VectorKey left, VectorKey right)
        {
            return !left.Equals(right);
        }
    }
}