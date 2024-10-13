using TrafficCamera.Shared;

namespace TrafficCamera;

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
            var road = parts[1];
            var licensePlate = parts[2];
            var speed = float.Parse(parts[4]);

            if (!dictionary.TryGetValue(road, out var accumulator))
            {
                dictionary[road] = accumulator = new Accumulator(road);
                accumulator.Record(speed, licensePlate);
            }
            line = reader.ReadLine();
        }

        foreach (var accumulator in dictionary.Values.OrderBy(a => a.Road))
        {
            Console.WriteLine($"{accumulator.Road}: {accumulator.Slowest:F1} [{accumulator.SlowestLicensePlate}]/{accumulator.Mean:F1}/{accumulator.Fastest:F1} [{accumulator.FastestLicensePlate}]");
        }

        return default;
    }
}
