using System;
using System.Collections.Generic;

public partial class ByteUtils
{
    public static byte[] toByteArray(int value)
    {
        return new byte[] {
                (byte)(value >> 24),
                (byte)(value >> 16),
                (byte)(value >> 8),
                (byte)value };
    }

    public static byte[] toByteArray(short value)
    {
            return new byte[] { (byte)(value & 0x00FF), (byte)((value & 0xFF00) >> 8) };
    }

    public static int[] SplitMask(int mask)
    {
        if (mask == 0) return Array.Empty<int>();
        var list = new List<int>();
        while (mask != 0)
        {
            int bit = mask & -mask; 
            list.Add(bit);
            mask &= mask - 1; 
        }
        return list.ToArray();
    }

    public static long[] SplitMask(long mask)
    {
        if (mask == 0L) return Array.Empty<long>();
        var list = new List<long>();
        while (mask != 0L)
        {
            long bit = mask & -mask;
            list.Add(bit);
            mask &= mask - 1;
        }
        return list.ToArray();
    }
}
