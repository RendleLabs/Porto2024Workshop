namespace TrafficCamera;

public ref struct ByteSpanEnumerator
{
    private readonly ReadOnlySpan<byte> _span;
    private readonly byte _delimiter;
    private int _offset = -1;
    private int _length;

    public ByteSpanEnumerator(ReadOnlySpan<byte> span, byte delimiter)
    {
        _span = span;
        _delimiter = delimiter;
    }

    public bool MoveNext()
    {
        // DATE;A24;GU23KII;Red;42.5
        // 0123456789012345678901234
        // o = 0, l = 4

        if (_offset == int.MinValue) return false;
        
        if (_offset == -1)
        {
            _offset = 0;
            int d = _span.IndexOf(_delimiter);
            if (d < 0)
            {
                _length = _span.Length;
                _offset = int.MinValue;
                return true;
            }

            _length = d;
            return true;
        }

        _offset += _length + 1;

        {
            int d = _span.Slice(_offset).IndexOf(_delimiter);
            if (d < 0)
            {
                _length = _span.Slice(_offset).Length;
                return true;
            }

            _length = d;
            return true;
        }
    }
    
    public ReadOnlySpan<byte> Current => _span.Slice(_offset, _length);
}