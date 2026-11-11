using System;
using System.Collections.Generic;

public partial class ByteUtils
{
    public static  int fromByteArray(byte[] bytes)
    {
        return bytes[0] << 24 | (bytes[1] & 0xFF) << 16 | (bytes[2] & 0xFF) << 8 | (bytes[3] & 0xFF);
    }

    public static double[] byte2Double(byte[] inData, bool byteSwap)
    {
        int j = 0, upper, lower;
        int length = inData.Length / 8;
        double[] outData = new double[length];
        if (!byteSwap)
            for (int i = 0; i < length; i++)
            {
                j = i * 8;
                upper = (((inData[j] & 0xff) << 24)
                    + ((inData[j + 1] & 0xff) << 16)
                    + ((inData[j + 2] & 0xff) << 8) + ((inData[j + 3] & 0xff) << 0));
                lower = (((inData[j + 4] & 0xff) << 24)
                    + ((inData[j + 5] & 0xff) << 16)
                    + ((inData[j + 6] & 0xff) << 8) + ((inData[j + 7] & 0xff) << 0));
                //outData[i] = Double.longBitsToDouble((((long)upper) << 32)
                outData[i] = (((long)upper) << 32);
            }
        else
            for (int i = 0; i < length; i++)
            {
                j = i * 8;
                upper = (((inData[j + 7] & 0xff) << 24)
                    + ((inData[j + 6] & 0xff) << 16)
                    + ((inData[j + 5] & 0xff) << 8) + ((inData[j + 4] & 0xff) << 0));
                lower = (((inData[j + 3] & 0xff) << 24)
                    + ((inData[j + 2] & 0xff) << 16)
                    + ((inData[j + 1] & 0xff) << 8) + ((inData[j] & 0xff) << 0));
                outData[i] = (((long)upper) << 32);
            }

        return outData;
    }

    public static short fromByteArrayShort(byte[] bytes)
    {
        return (short)(((bytes[0] & 0xFF) << 8) | (bytes[1] & 0xFF));
    }

    public static byte[] shortToBytes(short value)
    {
        return BitConverter.GetBytes(value);
    }
}
