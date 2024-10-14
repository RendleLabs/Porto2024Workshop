using System.Text;
using TrafficCamera.Shared;

namespace TrafficCamera;

public class StreamSpanEnumeratorImpl
{
    private readonly string _filePath;

    public StreamSpanEnumeratorImpl(string eFilePath)
    {
        _filePath = eFilePath;
    }

    public ValueTask Run()
    {
        var dictionary = new Dictionary<RoadKey, Accumulator>();
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

    private static void ParseLine(ReadOnlySpan<byte> line, Dictionary<RoadKey, Accumulator> dictionary)
    {
        var enumerator = new ByteSpanEnumerator(line, (byte)';');

        enumerator.MoveNext();
        enumerator.MoveNext();

        var road = enumerator.Current;
        var key = RoadKey.Create(road);

        enumerator.MoveNext();
        var licensePlate = enumerator.Current;

        enumerator.MoveNext();
        enumerator.MoveNext();

        var speedSpan = enumerator.Current;
        var speed = float.Parse(speedSpan);

        if (!dictionary.TryGetValue(key, out var accumulator))
        {
            dictionary[key] = accumulator = new Accumulator(Encoding.UTF8.GetString(road));
        }
        accumulator.Record(speed, Encoding.UTF8.GetString(licensePlate));
    }
}