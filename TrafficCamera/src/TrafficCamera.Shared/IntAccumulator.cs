using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Text;

namespace TrafficCamera.Shared;

[InlineArray(8)]
public struct ColorCounter
{
    public int Element0;
}

public class IntAccumulator
{
    public string Road { get; }
    private long _total;
    private int _count;
    private int _slowest = int.MaxValue;
    private string _slowestLicensePlate = string.Empty;
    private int _fastest = int.MinValue;
    private string _fastestLicensePlate = string.Empty;
    public ColorCounter ColorCounter = new();

    public IntAccumulator(string road)
    {
        Road = road;
    }

    public void Record(int value, ReadOnlySpan<byte> licensePlate, ReadOnlySpan<byte> color)
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

        int colorIndex = ColorIndex(color);
        ColorCounter[colorIndex] += 1;

        _total += value;
        ++_count;
    }

    private static int ColorIndex(ReadOnlySpan<byte> color)
    {
        switch (color[0])
        {
            case (byte)'R':
                return 0;
            case (byte)'G' when color.SequenceEqual("Green"u8):
                return 1;
            case (byte)'B' when color.SequenceEqual("Blue"u8):
                return 2;
            case (byte)'B' when color.SequenceEqual("Black"u8):
                return 3;
            case (byte)'W':
                return 4;
            case (byte)'G' when color.SequenceEqual("Grey"u8):
                return 5;
            case (byte)'S':
                return 6;
            default:
                return 7;
        }
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