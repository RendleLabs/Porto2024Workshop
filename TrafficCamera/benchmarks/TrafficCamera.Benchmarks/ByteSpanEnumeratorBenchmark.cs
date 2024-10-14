using BenchmarkDotNet.Attributes;

namespace TrafficCamera.Benchmarks;

public class ByteSpanEnumeratorBenchmark
{
    private static readonly ReadOnlyMemory<byte> Data = "2024-10-14T09:02:29.861;A117;JJ88 LBP;Silver;79.1"u8.ToArray();
    
    [Benchmark(Baseline = true)]
    public int Manual()
    {
        var line = Data.Span;
        
        var sc = line.IndexOf((byte)';');
        line = line.Slice(sc + 1); // Skip past the date/time
        sc = line.IndexOf((byte)';');
        
        var road = line.Slice(0, sc);
        
        line = line.Slice(sc + 1); // Skip past the road
        sc = line.IndexOf((byte)';');
        var licensePlate = line.Slice(0, sc);
        
        sc = line.LastIndexOf((byte)';'); // Find the last field (speed)
        var speedSpan = line.Slice(sc + 1);
        
        return road.Length + licensePlate.Length + speedSpan.Length;
    }

    [Benchmark]
    public int Enumerator()
    {
        var line = Data.Span;

        var e = new ByteSpanEnumerator(line, (byte)';');

        int t = 0;

        e.MoveNext();
        e.MoveNext();
        t += e.Current.Length;
        
        e.MoveNext();
        t += e.Current.Length;
        
        e.MoveNext();
        e.MoveNext();
        t += e.Current.Length;

        return t;
    }

    [Benchmark]
    public int SpanEnumerator()
    {
        var e = Data.Span.Split((byte)';');
        
        int t = 0;

        e.MoveNext();
        e.MoveNext();
        t += e.Current.End.Value - e.Current.Start.Value;
        
        e.MoveNext();
        t += e.Current.End.Value - e.Current.Start.Value;
        
        e.MoveNext();
        e.MoveNext();
        t += e.Current.End.Value - e.Current.Start.Value;

        return t;
    }
}