using TrafficCamera.Generator;

const int lines = 10_000_000;

using var writer = File.CreateText("traffic.txt");

var licensePlates = Enumerable.Range(0, 10_000)
    .Select(_ => RandomLicensePlate())
    .ToArray();

var roads = Enumerable.Range(1, 500)
    .Select(n => $"A{n}")
    .Concat(Enumerable.Range(1, 100).Select(n => $"M{n}"))
    .ToArray();

string[] colors = ["Red", "Green", "Blue", "Black", "White", "Grey", "Silver", "Other"];

for (int i = 0; i < lines; i++)
{
    writer.Write(RandomTime());
    writer.Write(';');
    writer.Write(Random.Shared.Take(roads));
    writer.Write(';');
    writer.Write(Random.Shared.Take(licensePlates));
    writer.Write(';');
    writer.Write(Random.Shared.Take(colors));
    writer.Write(';');
    writer.Write(RandomSpeed());
    writer.Write('\n');
}

string RandomTime()
{
    var hour = Random.Shared.Next(24);
    var minute = Random.Shared.Next(60);
    var second = Random.Shared.Next(60);
    var fraction = Random.Shared.Next(1000);
    
    return DateTimeOffset.UtcNow.Date
        .AddHours(hour)
        .AddMinutes(minute)
        .AddSeconds(second)
        .AddMilliseconds(fraction)
        .ToString("yyyy-MM-ddTHH:mm:ss.fff");
}
string RandomLicensePlate() => $"{Letter()}{Letter()}{Number()} {Letter()}{Letter()}{Letter()}";
string RandomSpeed() => (Random.Shared.NextSingle() * 50f + 30f).ToString("F1");

char Letter() => (char)Random.Shared.Next(65, 91);
string Number() => Random.Shared.Next(01, 100).ToString();