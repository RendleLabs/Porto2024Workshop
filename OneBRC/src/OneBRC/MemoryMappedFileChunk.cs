namespace OneBRC;

public readonly struct MemoryMappedFileChunk
{
    public readonly long Offset;
    public readonly long Length;
    public readonly int Index;

    public MemoryMappedFileChunk(long offset, long length, int index)
    {
        Offset = offset;
        Length = length;
        Index = index;
    }
}