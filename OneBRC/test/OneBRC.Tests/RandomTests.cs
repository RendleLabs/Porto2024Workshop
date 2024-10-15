using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;

namespace OneBRC.Tests;

public class RandomTests
{
    private readonly long _0 = Random.Shared.NextInt64();
    private readonly long _1 = Random.Shared.NextInt64();
    private readonly long _2 = Random.Shared.NextInt64();
    private readonly long _3 = Random.Shared.NextInt64();
    
    [Fact]
    public void StraightToSpan()
    {
        Span<long> span = stackalloc long[] { _0, _1, _2, _3 };
        var bytes = MemoryMarshal.AsBytes(span);

        Vector256.Create((ReadOnlySpan<long>)span);
    }
}