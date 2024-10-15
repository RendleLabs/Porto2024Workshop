using System.Diagnostics;
using OneBRC;

var stopwatch = Stopwatch.StartNew();

var filePath = Path.GetFullPath(args[0]);
var impl = new MemoryMappedFileImpl(filePath);
var task = impl.Run();

if (!task.IsCompleted)
{
    await task;
}

stopwatch.Stop();

Console.WriteLine();
Console.WriteLine($"Done in {stopwatch.Elapsed:g}");
Console.WriteLine();
