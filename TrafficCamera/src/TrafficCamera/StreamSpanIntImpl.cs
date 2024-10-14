using System.Text;
using TrafficCamera.Shared;

namespace TrafficCamera;

public class StreamSpanIntImpl
{
    private readonly string _filePath;

    public StreamSpanIntImpl(string eFilePath)
    {
        _filePath = eFilePath;
    }

    public ValueTask Run()
    {
        var dictionary = new Dictionary<RoadKey, IntAccumulator>();
        var buffer = new byte[1024];
        int offset = 0;

        using var stream = File.OpenRead(_filePath);
        int bytesRead;

        while ((bytesRead = stream.Read(buffer.AsSpan(offset))) > 0)
        {
            var span = buffer.AsSpan(0, bytesRead + offset);

            while (span.Length > 0)
            {
                var eol = span.IndexOf((byte)'\n');
                if (eol < 0) break;
                
                var line = span.Slice(0, eol);
                ParseLine(line, dictionary);
                span = span.Slice(eol + 1);
            }

            span.CopyTo(buffer);
            offset = span.Length;
        }
        
        foreach (var accumulator in dictionary.Values.OrderBy(a => a.Road))
        {
            Console.WriteLine($"{accumulator.Road}: {accumulator.Slowest:F1} [{accumulator.SlowestLicensePlate}]/{accumulator.Mean:F1}/{accumulator.Fastest:F1} [{accumulator.FastestLicensePlate}]");
        }

        return default;
    }

    private static void ParseLine(ReadOnlySpan<byte> line, Dictionary<RoadKey, IntAccumulator> dictionary)
    {
        var sc = line.IndexOf((byte)';');
        line = line.Slice(sc + 1); // Skip past the date/time
        sc = line.IndexOf((byte)';');
        
        var road = line.Slice(0, sc);
        var key = RoadKey.Create(road);
        
        line = line.Slice(sc + 1); // Skip past the road
        sc = line.IndexOf((byte)';');
        var licensePlate = line.Slice(0, sc);
        
        sc = line.LastIndexOf((byte)';'); // Find the last field (speed)
        var speedSpan = line.Slice(sc + 1);
        var speed = ParseSpeed(speedSpan);

        if (!dictionary.TryGetValue(key, out var accumulator))
        {
            dictionary[key] = accumulator = new IntAccumulator(Encoding.UTF8.GetString(road));
        }
        accumulator.Record(speed, licensePlate);
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