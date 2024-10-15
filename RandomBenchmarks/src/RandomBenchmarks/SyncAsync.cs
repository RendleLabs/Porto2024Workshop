using BenchmarkDotNet.Attributes;

namespace RandomBenchmarks;

[MemoryDiagnoser]
public class SyncAsync
{
    private static readonly int[] Values = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20];

    [Benchmark(Baseline = true)]
    public int Sync()
    {
        int t = 0;

        for (int i = 0, l = Values.Length; i < l; i++)
        {
            t += Values[i];
        }

        return t;
    }

    [Benchmark]
    public async Task<int> Async() => await AsyncImpl();

    [Benchmark]
    public async Task<int> AsyncOpt()
    {
        var t = AsyncImpl();
        if (t.IsCompleted) return t.Result;
        return await t;
    }
    
    private Task<int> AsyncImpl()
    {
        int t = 0;

        for (int i = 0, l = Values.Length; i < l; i++)
        {
            t += Values[i];
        }

        return Task.FromResult(t);
    }

    [Benchmark]
    public async ValueTask<int> ValueAsync() => await ValueAsyncImpl();

    [Benchmark]
    public ValueTask<int> ValueAsyncOpt()
    {
        var t = ValueAsyncImpl();
        if (t.IsCompleted) return t;

        return new ValueTask<int>(Await(t));

        static async Task<int> Await(ValueTask<int> vt)
        {
            return await vt;
        }
    }

    private ValueTask<int> ValueAsyncImpl()
    {
        int t = 0;

        for (int i = 0, l = Values.Length; i < l; i++)
        {
            t += Values[i];
        }

        return new ValueTask<int>(t);
    }
}