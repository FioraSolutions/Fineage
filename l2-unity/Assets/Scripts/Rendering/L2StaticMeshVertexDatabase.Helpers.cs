using System;
using UnityEngine;

public static partial class L2StaticMeshVertexDatabase
{
    static Vector3[] DecodeVectors(string encoded, int count, bool position)
    {
        byte[] bytes = Convert.FromBase64String(encoded ?? "");
        if (bytes.Length != count * 12)
            throw new InvalidOperationException(
                "Static-mesh vector buffer has " + bytes.Length + " bytes; expected " + (count * 12) + ".");

        var result = new Vector3[count];
        for (int i = 0; i < count; i++)
        {
            int p = i * 12;
            float x = ReadSingleLittleEndian(bytes, p);
            float y = ReadSingleLittleEndian(bytes, p + 4);
            float z = ReadSingleLittleEndian(bytes, p + 8);
            Vector3 converted = new Vector3(-x, z, y);
            result[i] = position
                ? converted * StaticMeshFbxScale
                : converted.normalized;
        }
        return result;
    }

    static float ReadSingleLittleEndian(byte[] bytes, int offset)
    {
        if (BitConverter.IsLittleEndian)
            return BitConverter.ToSingle(bytes, offset);
        var value = new byte[4];
        value[0] = bytes[offset + 3];
        value[1] = bytes[offset + 2];
        value[2] = bytes[offset + 1];
        value[3] = bytes[offset];
        return BitConverter.ToSingle(value, 0);
    }
}
