using System.Runtime.Intrinsics;
using BenchmarkDotNet.Attributes;
using TrafficCamera.Shared;

namespace TrafficCamera.Benchmarks;

public class ColorCounters
{
    private static readonly ColorCounter[] _colorCounters = Generate().ToArray();

    [Benchmark(Baseline = true)]
    public int Manual()
    {
        var total = new ColorCounter();

        for (int i = 0; i < _colorCounters.Length; i++)
        {
            total[0] += _colorCounters[i][0];
            total[1] += _colorCounters[i][1];
            total[2] += _colorCounters[i][2];
            total[3] += _colorCounters[i][3];
            total[4] += _colorCounters[i][4];
            total[5] += _colorCounters[i][5];
            total[6] += _colorCounters[i][6];
            total[7] += _colorCounters[i][7];
        }
        
        return total[0] + total[1] + total[2] + total[3] + total[4] + total[5] + total[6] + total[7];
    }

    [Benchmark]
    public int Vector()
    {
        var total = Vector256<int>.Zero;

        for (int i = 0; i < _colorCounters.Length; i++)
        {
            total = Vector256.Add(total, Vector256.Create<int>(_colorCounters[i]));
        }
        
        return total[0] + total[1] + total[2] + total[3] + total[4] + total[5] + total[6] + total[7];
    }
    private static IEnumerable<ColorCounter> Generate()
    {
        for (int i = 0; i < 1024; i++)
        {
            var colorCounter = new ColorCounter();
            colorCounter[0] = Random.Shared.Next(32);
            colorCounter[1] = Random.Shared.Next(32);
            colorCounter[2] = Random.Shared.Next(32);
            colorCounter[3] = Random.Shared.Next(32);
            colorCounter[4] = Random.Shared.Next(32);
            colorCounter[5] = Random.Shared.Next(32);
            colorCounter[6] = Random.Shared.Next(32);
            colorCounter[7] = Random.Shared.Next(32);

            yield return colorCounter;
        }
    }
}