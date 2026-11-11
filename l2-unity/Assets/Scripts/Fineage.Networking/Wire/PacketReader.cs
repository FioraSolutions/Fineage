using System;
using System.Text;
public sealed partial class PacketReader
{
    private readonly byte[] _data;
    private int _iterator;


    public int Remaining
    {
        get { return _data == null ? 0 : _data.Length - _iterator; }
    }














}
