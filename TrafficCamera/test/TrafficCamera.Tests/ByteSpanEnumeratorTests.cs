namespace TrafficCamera.Tests;

public class ByteSpanEnumeratorTests
{
    [Fact]
    public void SplitsCorrectly()
    {
        var src = "2024-10-14T09:02:29.861;A117;JJ88 LBP;Silver;79.1"u8;

        var target = new ByteSpanEnumerator(src, (byte)';');
        
        Assert.True(target.MoveNext());
        Assert.True(target.Current.SequenceEqual("2024-10-14T09:02:29.861"u8));
        
        Assert.True(target.MoveNext());
        Assert.True(target.Current.SequenceEqual("A117"u8));
        
        Assert.True(target.MoveNext());
        Assert.True(target.Current.SequenceEqual("JJ88 LBP"u8));
        
        Assert.True(target.MoveNext());
        Assert.True(target.Current.SequenceEqual("Silver"u8));
        
        Assert.True(target.MoveNext());
        Assert.True(target.Current.SequenceEqual("79.1"u8));
    }
}
