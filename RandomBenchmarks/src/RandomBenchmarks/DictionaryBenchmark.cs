using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace RandomBenchmarks;

public class DictionaryBenchmark
{
    private static readonly Dictionary<string, int> Counts = new()
    {
        ["A"] = 1
    };

    [Benchmark(Baseline = true)]
    public int TryGetValue()
    {
        if (Counts.TryGetValue("A", out var count))
        {
            Counts["A"] = count + 1;
        }
        else
        {
            Counts["A"] = 1;
        }

        return Counts["A"];
    }
    
    [Benchmark]
    public int Marshalling() => CollectionsMarshal.GetValueRefOrAddDefault(Counts, "A", out _) += 1;
}