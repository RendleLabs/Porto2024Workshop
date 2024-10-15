using System.IO.MemoryMappedFiles;
using System.Text;
using OneBRC.Shared;

namespace OneBRC;

public class ChunkParser
{
    private readonly MemoryMappedViewAccessor _view;
    private readonly MemoryMappedFileChunk _chunk;

    public ChunkParser(MemoryMappedViewAccessor view, MemoryMappedFileChunk chunk)
    {
        _view = view;
        _chunk = chunk;
    }

    public Dictionary<CityKey, IntAccumulator> Aggregate { get; } = new();

    public unsafe void Run()
    {
        var chunkLength = (int)_chunk.Length;
        byte* ptr = null;
        _view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
        var span = new ReadOnlySpan<byte>(ptr + _chunk.Offset, chunkLength);
        Run(span);
    }

    private void Run(ReadOnlySpan<byte> span)
    {
        while (span.Length > 0)
        {
            var eol = span.IndexOf((byte)'\n');
            if (eol < 0) break;
            
            var line = span.Slice(0, eol);
            Process(line);
            span = span.Slice(eol + 1);
        }
    }

    private void Process(ReadOnlySpan<byte> line)
    {
        var semicolon = line.IndexOf((byte)';');
        var name = line.Slice(0, semicolon);
        var temp = line.Slice(semicolon + 1);

        var key = CityKey.Create(name);
        var temperature = ParseTemperature(temp);

        if (!Aggregate.TryGetValue(key, out var accumulator))
        {
            Aggregate[key] = accumulator = new(Encoding.UTF8.GetString(name));
        }
        accumulator.Record(temperature);
    }
    
    private static int ParseTemperature(ReadOnlySpan<byte> span)
    {
        int multiplier = 1;
        if (span[0] == (byte)'-')
        {
            multiplier = -1;
            span = span.Slice(1);
        }

        int total = 0;

        while (span.Length > 0)
        {
            if (span[0] is > 47 and < 58)
            {
                total = (total * 10) + (span[0] - 48);
            }

            span = span.Slice(1);
        }

        return total * multiplier;
    }
}