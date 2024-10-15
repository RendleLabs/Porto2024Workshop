using System.Collections.Frozen;
using BenchmarkDotNet.Attributes;

namespace RandomBenchmarks;

public class FreezingBenchmark
{
    private static readonly Dictionary<string, int> Dict =
        Enumerable.Range(0, 10000).ToDictionary(n => n.ToString());
    
    private static readonly FrozenDictionary<string, int> FrozenDict =
        Enumerable.Range(0, 10000).ToFrozenDictionary(n => n.ToString());

    [Benchmark(Baseline = true)]
    public int Dictionary() => Dict.GetValueOrDefault("9999", -1);
    
    [Benchmark]
    public int FrozenDictionary() => FrozenDict.GetValueOrDefault("9999", -1);
}