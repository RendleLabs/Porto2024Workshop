using System.Diagnostics;
using TrafficCamera;

var stopwatch = Stopwatch.StartNew();

var filePath = Path.GetFullPath(args[0]);

var impl = new StreamSpanImpl(filePath);
var t = impl.Run();

if (!t.IsCompleted)
{
    await t;
}

stopwatch.Stop();

Console.WriteLine(stopwatch.Elapsed);
