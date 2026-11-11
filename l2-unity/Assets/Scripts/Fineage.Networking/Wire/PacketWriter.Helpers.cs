using System;
using System.Collections.Generic;
using System.Text;

public sealed partial class PacketWriter
{
    public void InsertOpcode(byte opcode)
    {
        _buffer.Insert(0, opcode);
    }

    public void WriteZeroInt()
    {
        WriteI(0);
    }

    public void Pad8()
    {
        int paddingLength = _buffer.Count % 8;
        if (paddingLength == 0)
            return;

        paddingLength = 8 - paddingLength;
        for (int i = 0; i < paddingLength; i++)
            _buffer.Add(0);
    }

    public byte[] ToArray()
    {
        return _buffer.ToArray();
    }

    public byte[] BuildGame(byte opcode)
    {
        InsertOpcode(opcode);
        WriteZeroInt();
        Pad8();
        return ToArray();
    }

    public byte[] BuildGameNoPad(byte opcode)
    {
        InsertOpcode(opcode);
        return ToArray();
    }

    public byte[] BuildGameOverwriteOpcode(byte opcode)
    {
        if (_buffer.Count == 0)
            WriteB(opcode);
        else
            _buffer[0] = opcode;
        WriteZeroInt();
        Pad8();
        return ToArray();
    }

    static byte[] ToByteArray(int value)
    {
        return new byte[] {
            (byte)(value >> 24),
            (byte)(value >> 16),
            (byte)(value >> 8),
            (byte)value };
    }

    static byte[] ToByteArray(short value)
    {
        return new byte[] { (byte)(value & 0x00FF), (byte)((value & 0xFF00) >> 8) };
    }
}
