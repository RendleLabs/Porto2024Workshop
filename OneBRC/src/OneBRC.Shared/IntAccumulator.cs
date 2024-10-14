namespace OneBRC.Shared;

public class IntAccumulator
{
    public string City { get; }
    private long _total;
    private int _count;
    private int _min = int.MaxValue;
    private int _max = int.MinValue;

    public IntAccumulator(string city)
    {
        City = city;
    }

    public void Record(int value)
    {
        if (value < _min) _min = value;
        if (value > _max) _max = value;
        _total += value;
        ++_count;
    }
    
    public void Combine(IntAccumulator other)
    {
        _count += other._count;
        _total += other._total;
        if (other._min < _min) _min = other._min;
        if (other._max < _max) _max = other._max;
    }

    public float Mean => (_total / 1000f) / _count;
    public float Min => _min / 1000f;
    public float Max => _max / 1000f;
    public int Count => _count;
}