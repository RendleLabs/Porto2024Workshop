using OneBRC.Shared;

namespace OneBRC;

public class StreamReaderImpl
{
    private readonly string _filePath;

    public StreamReaderImpl(string filePath)
    {
        _filePath = filePath;
    }

    public ValueTask Run()
    {
        var dictionary = new Dictionary<string, Accumulator>();
        using var reader = File.OpenText(_filePath);

        var line = reader.ReadLine();
        while (line is {Length: > 0})
        {
            var parts = line.Split(';');
            var city = parts[0];
            var value = float.Parse(parts[1]);

            if (!dictionary.TryGetValue(city, out var accumulator))
            {
                dictionary[city] = accumulator = new Accumulator(city);
                accumulator.Record(value);
            }
            line = reader.ReadLine();
        }

        foreach (var accumulator in dictionary.Values.OrderBy(a => a.City))
        {
            Console.WriteLine($"{accumulator.City}: {accumulator.Min:F1}/{accumulator.Mean:F1}/{accumulator.Max:F1}");
        }

        return default;
    }
}