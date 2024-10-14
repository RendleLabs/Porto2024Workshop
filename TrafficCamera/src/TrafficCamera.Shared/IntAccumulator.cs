using System.Text;

namespace TrafficCamera.Shared;

public class IntAccumulator
{
    public string Road { get; }
    private long _total;
    private int _count;
    private int _slowest = int.MaxValue;
    private string _slowestLicensePlate = string.Empty;
    private int _fastest = int.MinValue;
    private string _fastestLicensePlate = string.Empty;

    public IntAccumulator(string road)
    {
        Road = road;
    }

    public void Record(int value, ReadOnlySpan<byte> licensePlate)
    {
        if (value < _slowest)
        {
            _slowest = value;
            _slowestLicensePlate = Encoding.UTF8.GetString(licensePlate);
        }

        if (value > _fastest)
        {
            _fastest = value;
            _fastestLicensePlate = Encoding.UTF8.GetString(licensePlate);
        }

        _total += value;
        ++_count;
    }
    
    public void Combine(IntAccumulator other)
    {
        _count += other._count;
        _total += other._total;
        if (other._slowest < _slowest)
        {
            _slowest = other._slowest;
            _slowestLicensePlate = other._slowestLicensePlate;
        }

        if (other._fastest < _fastest)
        {
            _fastest = other._fastest;
            _fastestLicensePlate = other._fastestLicensePlate;
        }
    }

    public float Mean => (_total / 10f) / _count;
    public float Slowest => _slowest / 10f;
    public string SlowestLicensePlate => _slowestLicensePlate;
    public float Fastest => _fastest / 10f;
    public string FastestLicensePlate => _fastestLicensePlate;
}