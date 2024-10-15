using System.IO.MemoryMappedFiles;
using OneBRC.Shared;

namespace OneBRC;

public class MemoryMappedFileImpl
{
    private readonly string _filePath;

    public MemoryMappedFileImpl(string filePath)
    {
        _filePath = filePath;
    }

    public ValueTask Run()
    {
        var size = new FileInfo(_filePath).Length;
        var mmf = MemoryMappedFile.CreateFromFile(_filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
        var view = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);

        var chunks = mmf.GetChunks(size, Environment.ProcessorCount);
        
        var parsers = chunks
            .Select(c => new ChunkParser(view, c))
            .ToArray();
        
        var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };

        Parallel.ForEach(parsers, options, p => p.Run());

        var result = new Dictionary<string, IntAccumulator>();

        foreach (var parser in parsers)
        {
            foreach (var value in parser.Aggregate.Values)
            {
                if (result.TryGetValue(value.City, out var accumulator))
                {
                    accumulator.Combine(value);
                }
                else
                {
                    result[value.City] = value;
                }
            }
        }
        
        foreach (var accumulator in result.Values.OrderBy(a => a.City))
        {
            Console.WriteLine($"{accumulator.City}: {accumulator.Min:F1}/{accumulator.Mean:F1}/{accumulator.Max:F1}");
        }

        return default;
    }
}