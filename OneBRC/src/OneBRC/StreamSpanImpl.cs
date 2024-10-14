using System.Text;
using OneBRC.Shared;

namespace OneBRC;

public class StreamSpanImpl
{
    private readonly string _filePath;

    public StreamSpanImpl(string filePath)
    {
        _filePath = filePath;
    }

    public ValueTask Run()
    {
        var dict = new Dictionary<CityKey, Accumulator>();
        var buffer = new byte[1024];
        int offset = 0;

        var stream = File.OpenRead(_filePath);

        int bytesRead;

        while ((bytesRead = stream.Read(buffer.AsSpan(offset))) > 0)
        {
            var span = buffer.AsSpan(0, offset + bytesRead);

            while (span.Length > 0)
            {
                int eol = span.IndexOf((byte)'\n');

                if (eol > 0)
                {
                    var line = span.Slice(0, eol);
                    ParseLine(line, dict);
                    span = span.Slice(eol + 1);
                }
                else
                {
                    break;
                }
            }

            span.CopyTo(buffer);
            offset = span.Length;
        }
        
        foreach (var accumulator in dict.Values.OrderBy(a => a.City))
        {
            Console.WriteLine($"{accumulator.City}: {accumulator.Min:F1}/{accumulator.Mean:F1}/{accumulator.Max:F1}");
        }

        return default;
    }

    private static void ParseLine(ReadOnlySpan<byte> line, Dictionary<CityKey, Accumulator> dict)
    {
        // Porto;25.345
        int semicolon = line.IndexOf((byte)';');
        var city = line.Slice(0, semicolon);
        var key = CityKey.Create(city);
        var temperature = line.Slice(semicolon + 1);

        float floatTemp;
        try
        {
            floatTemp = float.Parse(temperature);
        }
        catch (Exception e)
        {
            Console.WriteLine();
            Console.WriteLine("OOPS");
            Console.WriteLine(Encoding.UTF8.GetString(temperature));
            throw;
        }

        if (!dict.TryGetValue(key, out var accumulator))
        {
            dict[key] = accumulator = new Accumulator(Encoding.UTF8.GetString(city));
        }
        
        accumulator.Record(floatTemp);
    }
}