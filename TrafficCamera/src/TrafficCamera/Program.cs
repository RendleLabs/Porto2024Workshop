using TrafficCamera;

var filePath = Path.GetFullPath(args[0]);

var impl = new StreamReaderImpl(filePath);
var t = impl.Run();

if (!t.IsCompleted)
{
    await t;
}
