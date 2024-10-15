using System.IO.MemoryMappedFiles;

namespace OneBRC;

public static class MemoryMappedFileAnalyzer
{
    public static unsafe MemoryMappedFileChunk[] GetChunks(this MemoryMappedFile mmf, long size, int threadCount)
    {
        var chunks = new MemoryMappedFileChunk[threadCount];
        
        var accessor = mmf.CreateViewAccessor(0, size, MemoryMappedFileAccess.Read);
        byte* pointer = null;
        accessor.SafeMemoryMappedViewHandle.AcquirePointer(ref pointer);

        var estimatedChunkSize = (int)(size / threadCount);
        
        long offset = 0;

        for (int i = 0; i < threadCount - 1; i++)
        {
            var span = new ReadOnlySpan<byte>(pointer + offset, estimatedChunkSize);
            var lastNewline = span.LastIndexOf((byte)'\n');
            var actualSize = lastNewline + 1;
            chunks[i] = new MemoryMappedFileChunk(offset, actualSize, i);
            offset += actualSize;
        }
        
        var last = new MemoryMappedFileChunk(offset, size - offset, threadCount - 1);
        chunks[^1] = last;

        return chunks;
    }
}