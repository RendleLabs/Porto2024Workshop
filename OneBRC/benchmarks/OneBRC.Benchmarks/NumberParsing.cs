using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

public class NumberParsing
{
    private static readonly ReadOnlyMemory<byte> Data = "27.253"u8.ToArray();
    
    [Benchmark(Baseline = true)]
    public float ParseFloat()
    {
        return float.Parse(Data.Span);
    }

    [Benchmark]
    public int ParseInt()
    {
        int value = 0;
        
        var span = Data.Span;
        while (span.Length > 0)
        {
            if (span[0] is > 47 and < 59)
            {
                value = (value * 10) + (span[0] - 48);
            }
            span = span.Slice(1);
        }

        return value;
    }
}