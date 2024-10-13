namespace TrafficCamera.Shared;

public class Accumulator
{
    public string Road { get; }
    private float _total;
    private int _count;
    private float _slowest = float.MaxValue;
    private string _slowestLicensePlate = string.Empty;
    private float _fastest = float.MinValue;
    private string _fastestLicensePlate = string.Empty;

    public Accumulator(string road)
    {
        Road = road;
    }

    public void Record(float value, string licensePlate)
    {
        if (value < _slowest)
        {
            _slowest = value;
            _slowestLicensePlate = licensePlate;
        }

        if (value > _fastest)
        {
            _fastest = value;
            _fastestLicensePlate = licensePlate;
        }

        _total += value;
        ++_count;
    }
    
    public void Combine(Accumulator other)
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

    public float Mean => _total / _count;
    public float Slowest => _slowest;
    public string SlowestLicensePlate => _slowestLicensePlate;
    public float Fastest => _fastest;
    public string FastestLicensePlate => _fastestLicensePlate;
}