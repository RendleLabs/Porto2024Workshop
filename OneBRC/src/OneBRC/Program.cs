using System.Diagnostics;
using OneBRC;

// NEVER DO THIS
// Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.RealTime;
// UNLESS YOU REALLY WANT TO

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
