using System.IO.MemoryMappedFiles;
using System.Runtime.Intrinsics;
using TrafficCamera.Shared;

namespace TrafficCamera;

public class MemoryMappedFileImpl
{
    private readonly string _filePath;

    public MemoryMappedFileImpl(string eFilePath)
    {
        _filePath = eFilePath;
    }

    public ValueTask Run()
    {
        var size = new FileInfo(_filePath).Length;
        using var mmf = MemoryMappedFile.CreateFromFile(_filePath, FileMode.OpenOrCreate, null, 0, MemoryMappedFileAccess.Read);
        using var view = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);

        var chunks = mmf.GetChunks(size, Environment.ProcessorCount);
        
        var parsers = chunks
            .Select(c => new ChunkParser(view, c))
            .ToArray();
        
        var options = new ParallelOptions{MaxDegreeOfParallelism = Environment.ProcessorCount};

        Parallel.ForEach(parsers, options, p => p.Run());

        var result = new Dictionary<string, IntAccumulator>();

        foreach (var parser in parsers)
        {
            foreach (var value in parser.Data.Values)
            {
                if (result.TryGetValue(value.Road, out var accumulator))
                {
                    accumulator.Combine(value);
                }
                else
                {
                    result.Add(value.Road, value);
                }
            }
        }
        
        var totalColors = Vector256<int>.Zero;
        
        foreach (var accumulator in result.Values.OrderBy(a => a.Road))
        {
            Console.WriteLine($"{accumulator.Road}: {accumulator.Slowest:F1} [{accumulator.SlowestLicensePlate}]/{accumulator.Mean:F1}/{accumulator.Fastest:F1} [{accumulator.FastestLicensePlate}]");

            var colors = Vector256.Create<int>(accumulator.ColorCounter);
            
            totalColors = Vector256.Add(totalColors, colors);
        }

        Console.WriteLine();
        Console.WriteLine($"Red: {totalColors[0]}");
        Console.WriteLine($"Green: {totalColors[1]}");
        Console.WriteLine($"Blue: {totalColors[2]}");
        Console.WriteLine($"Black: {totalColors[3]}");
        Console.WriteLine($"White: {totalColors[4]}");
        Console.WriteLine($"Grey: {totalColors[5]}");
        Console.WriteLine($"Silver: {totalColors[6]}");
        Console.WriteLine($"Other: {totalColors[7]}");

        return default;
    }
}