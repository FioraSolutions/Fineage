using System;
using System.Text;
public sealed partial class PacketReader
{
    public PacketReader(byte[] data, bool skipOpcode = true)
    {
        _data = data;
        if (skipOpcode && _data != null && _data.Length > 0)
            ReadB();
    }

    public bool HasRemaining(int count)
    {
        return count <= 0 || Remaining >= count;
    }

    public byte ReadB()
    {
        if (_data == null || _iterator >= _data.Length)
            return 0;
        return _data[_iterator++];
    }

    public byte[] ReadB(int length)
    {
        byte[] data = new byte[length];
        Array.Copy(_data, _iterator, data, 0, length);
        _iterator += length;
        return data;
    }

    public int ReadH()
    {
        byte[] data = new byte[2];
        Array.Copy(_data, _iterator, data, 0, 2);
        double value = BitConverter.ToInt16(data, 0);
        _iterator += 2;
        return (int)value;
    }

    public int ReadI()
    {
        if (_iterator + 4 > _data.Length)
            return 0;

        byte[] data = new byte[4];
        Array.Copy(_data, _iterator, data, 0, 4);
        Array.Reverse(data);
        int value = FromByteArray(data);
        _iterator += 4;
        return value;
    }

    public int ReadSh()
    {
        if (_data == null || _iterator + 2 > _data.Length)
        {
            // Debug.LogError("Not enough bytes available to read short value");
            return 0;
        }

        byte[] data = new byte[2];
        Array.Copy(_data, _iterator, data, 0, 2);
        Array.Reverse(data);
        short value = FromByteArrayShort(data);
        _iterator += 2;
        return value;
    }
}
