using BenchmarkDotNet.Attributes;

namespace OneBRC.Benchmarks;

[MemoryDiagnoser(displayGenColumns: true)]
public class LineSplit
{
    private const string Str = "Porto;25.013";
    
    [Benchmark(Baseline = true)]
    public int StringSplit()
    {
        var parts = Str.Split(';');
        return parts[0].Length;
    }

    [Benchmark]
    public int SpanSplit()
    {
        Span<Range> ranges = stackalloc Range[2];
        var parts = Str.AsSpan().Split(ranges, ';');
        return ranges[0].End.Value - ranges[0].Start.Value;
    }
}