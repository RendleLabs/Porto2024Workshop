using OneBRC.Shared;

namespace OneBRC;

public class StreamReaderSpanImpl
{
    private readonly string _filePath;

    public StreamReaderSpanImpl(string filePath)
    {
        _filePath = filePath;
    }

    public ValueTask Run()
    {
        Span<Range> parts = stackalloc Range[2];
        var dictionary = new Dictionary<CityKey, Accumulator>();
        using var reader = File.OpenText(_filePath);

        var line = reader.ReadLine().AsSpan();
        while (line is {Length: > 0})
        {
            line.Split(parts, ';');
            
            var city = line[parts[0]];
            var key = CityKey.Create(city);
            
            var value = float.Parse(line[parts[1]]);

            if (!dictionary.TryGetValue(key, out var accumulator))
            {
                dictionary[key] = accumulator = new Accumulator(city.ToString());
            }
            accumulator.Record(value);
            line = reader.ReadLine().AsSpan();
        }

        foreach (var accumulator in dictionary.Values.OrderBy(a => a.City))
        {
            Console.WriteLine($"{accumulator.City}: {accumulator.Min:F1}/{accumulator.Mean:F1}/{accumulator.Max:F1}");
        }

        return default;
    }
}