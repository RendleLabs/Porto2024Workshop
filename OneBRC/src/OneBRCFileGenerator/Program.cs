using System.Diagnostics;
using System.Runtime.InteropServices;
using OneBRC.Shared;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: dotnet run -c Release -- 1000000000 ../../billion.txt");
    return -1;
}

int rows;
if (!int.TryParse(args[0], out rows))
{
    rows = 100;
}

var path = Path.GetFullPath(args[1]);

using var writer = File.CreateText(path);

int count = 0;

var dict = new Dictionary<string, float>();

foreach (var station in Stations.Randomize(rows))
{
    if (++count % 1000000 == 0)
    {
        Console.WriteLine($"{count}...");
    }

    ref var value = ref CollectionsMarshal.GetValueRefOrAddDefault(dict, station, out bool existed);
    if (!existed)
    {
        value = (Random.Shared.NextSingle() - 0.5f) * Random.Shared.Next(100);
    }
    
    var actual = value + (Random.Shared.NextSingle() - 0.5f) * Random.Shared.Next(10);
    
    writer.Write($"{station};{actual:F3}\n");
}

Console.WriteLine("Done.");

return 0;
