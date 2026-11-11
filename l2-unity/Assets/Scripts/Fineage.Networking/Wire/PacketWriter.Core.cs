using System;
using System.Collections.Generic;
using System.Text;

public sealed partial class PacketWriter
{
    public void WriteB(byte b)
    {
        _buffer.Add(b);
    }

    public void WriteB(byte[] b)
    {
        _buffer.AddRange(b);
    }

    public void WriteSOther(string s)
    {
        byte[] data = Encoding.GetEncoding("UTF-16").GetBytes(s ?? string.Empty);
        _buffer.Add((byte)data.Length);
        _buffer.AddRange(data);
    }

    public void WriteOtherS(string text)
    {
        if (!string.IsNullOrEmpty(text))
        {
            foreach (char c in text)
            {
                _buffer.Add((byte)(c & 0xFF));
                _buffer.Add((byte)(c >> 8));
            }
        }

        _buffer.Add(0);
        _buffer.Add(0);
    }

    public void WriteChar(char value)
    {
        short sho = (short)value;
        byte[] data = ToByteArray(sho);
        Array.Reverse(data);
        _buffer.AddRange(data);
    }

    public void WriteShort(short value)
    {
        byte[] data = ToByteArray(value);
        Array.Reverse(data);
        _buffer.AddRange(data);
    }

    public void WriteOtherShort(short value)
    {
        _buffer.AddRange(ToByteArray(value));
    }

    public void WriteI(int i)
    {
        byte[] data = ToByteArray(i);
        Array.Reverse(data);
        _buffer.AddRange(data);
    }

    public void WriteF(float i)
    {
        byte[] data = BitConverter.GetBytes(i);
        Array.Reverse(data);
        _buffer.AddRange(data);
    }
}
