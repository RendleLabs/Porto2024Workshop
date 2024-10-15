using System.IO.MemoryMappedFiles;
using System.Text;
using TrafficCamera.Shared;

namespace TrafficCamera;

public class ChunkParser
{
    private readonly MemoryMappedViewAccessor _view;
    private readonly MemoryMappedFileChunk _chunk;
    
    public readonly Dictionary<RoadKey, IntAccumulator> Data = new();

    public ChunkParser(MemoryMappedViewAccessor view, MemoryMappedFileChunk chunk)
    {
        _view = view;
        _chunk = chunk;
    }

    public unsafe void Run()
    {
        var chunkLength = (int)_chunk.Length;
        byte* ptr = null;
        _view.SafeMemoryMappedViewHandle.AcquirePointer(ref ptr);
        var span = new ReadOnlySpan<byte>(ptr, chunkLength);
        Parse(span);
    }

    private void Parse(ReadOnlySpan<byte> span)
    {
        while (span.Length > 0)
        {
            int newline = span.IndexOf((byte)'\n');
            if (newline < 0) break;
            
            var line = span.Slice(0, newline);
            ParseLine(line);
            span = span.Slice(newline + 1);
        }
    }
    
    private void ParseLine(ReadOnlySpan<byte> line)
    {
        var sc = line.IndexOf((byte)';');
        line = line.Slice(sc + 1); // Skip past the date/time
        sc = line.IndexOf((byte)';');
        
        var road = line.Slice(0, sc);
        var key = RoadKey.Create(road);
        
        line = line.Slice(sc + 1); // Skip past the road
        sc = line.IndexOf((byte)';');
        var licensePlate = line.Slice(0, sc);

        line = line.Slice(sc + 1);
        sc = line.IndexOf((byte)';');
        var color = line.Slice(0, sc);
        
        sc = line.LastIndexOf((byte)';'); // Find the last field (speed)
        var speedSpan = line.Slice(sc + 1);
        var speed = ParseSpeed(speedSpan);

        if (!Data.TryGetValue(key, out var accumulator))
        {
            Data[key] = accumulator = new IntAccumulator(Encoding.UTF8.GetString(road));
        }
        accumulator.Record(speed, licensePlate, color);
    }

    private static int ParseSpeed(ReadOnlySpan<byte> span)
    {
        int total = 0;

        while (span.Length > 0)
        {
            if (span[0] is > 47 and < 58)
            {
                total = (total * 10) + (span[0] - 48);
            }
            span = span.Slice(1);
        }

        return total;
    }
}