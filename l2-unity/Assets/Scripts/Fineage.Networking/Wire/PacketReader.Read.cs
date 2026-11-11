using System;
using System.Text;
public sealed partial class PacketReader
{
    public long ReadL()
    {
        byte[] data = new byte[8];
        Array.Copy(_data, _iterator, data, 0, 8);
        Array.Reverse(data);
        long value = BitConverter.ToInt64(data, 0);
        _iterator += 8;
        return value;
    }

    public long ReadLOther()
    {
        byte[] data = new byte[8];
        Array.Copy(_data, _iterator, data, 0, 8);
        long value = BitConverter.ToInt64(data, 0);
        _iterator += 8;
        return value;
    }

    public double ReadD()
    {
        byte[] data = new byte[8];
        Array.Copy(_data, _iterator, data, 0, 8);
        var value = BitConverter.ToDouble(data, 0);
        _iterator += 8;
        return value;
    }

    public float ReadF()
    {
        byte[] data = new byte[4];
        Array.Copy(_data, _iterator, data, 0, 4);
        Array.Reverse(data);
        float value = BitConverter.ToSingle(data, 0);
        _iterator += 4;
        return value;
    }

    public string ReadS()
    {
        byte strLen = ReadB();
        byte[] data = new byte[strLen];
        Array.Copy(_data, _iterator, data, 0, strLen);
        _iterator += strLen;
        return Encoding.GetEncoding("UTF-8").GetString(data);
    }

    public string ReadOtherS()
    {
        int strLen = 2;
        string text = "";
        try
        {
            for (int i = _iterator; i < _data.Length; i++)
            {
                byte[] data = new byte[strLen];
                Array.Copy(_data, _iterator, data, 0, strLen);
                Array.Reverse(data);
                char str = (char)FromByteArrayShort(data);
                _iterator += strLen;
                if (str == 0)
                    break;
                text += str;
            }
        }
        catch (Exception)
        {
            // Debug.Log("Serverpacket пришла пустая строка !!!");
            return "";
        }

        return text;
    }

    static int FromByteArray(byte[] bytes)
    {
        return bytes[0] << 24 | (bytes[1] & 0xFF) << 16 | (bytes[2] & 0xFF) << 8 | (bytes[3] & 0xFF);
    }

    static short FromByteArrayShort(byte[] bytes)
    {
        return (short)(((bytes[0] & 0xFF) << 8) | (bytes[1] & 0xFF));
    }
}
