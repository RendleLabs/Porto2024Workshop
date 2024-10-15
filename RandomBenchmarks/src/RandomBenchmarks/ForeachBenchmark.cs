using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;

namespace RandomBenchmarks;

public class ForeachBenchmark
{
    private static readonly List<int> NumberList = Enumerable.Range(0, 9999).ToList();
    private static readonly int[] NumberArray = Enumerable.Range(0, 9999).ToArray();

    [Benchmark(Baseline = true)]
    public int BasicForeach()
    {
        int total = 0;

        foreach (var number in NumberList)
        {
            total += number;
        }

        return total;
    }

    [Benchmark]
    public int BasicFor()
    {
        int total = 0;

        for (int i = 0, c = NumberList.Count; i < c; i++)
        {
            total += NumberList[i];
        }

        return total;
    }
    
    [Benchmark]
    public int ArrayForeach()
    {
        int total = 0;

        foreach (var number in NumberArray)
        {
            total += number;
        }

        return total;
    }

    [Benchmark]
    public int SpanForeach()
    {
        int total = 0;
        var span = CollectionsMarshal.AsSpan(NumberList);

        foreach (var number in span)
        {
            total += number;
        }

        return total;
    }
}