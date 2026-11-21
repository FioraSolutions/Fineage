using System;
using System.Buffers.Binary;
using Fineage.Net.Contract;

/// <summary>Client-side MonRaceInfo decode until Backend contract adds <see cref="NetCodec.DecodeMonRaceInfo"/>.</summary>
public static partial class NetZonePacketDecoder
{
    static MonRaceInfoNetMsg DecodeMonRaceInfo(ReadOnlySpan<byte> payload)
    {
        int i = 0;
        int codeA = ReadInt32(payload, ref i);
        int codeB = ReadInt32(payload, ref i);
        int n = ReadInt32(payload, ref i);
        if (n < 0)
            n = 0;
        var runners = new MonRaceRunnerNetMsg[n];
        for (int r = 0; r < n; r++)
        {
            var runner = new MonRaceRunnerNetMsg { ObjectId = ReadInt32(payload, ref i) };
            runner.NpcIdPlusDisplay = ReadInt32(payload, ref i);
            runner.OriginX = ReadInt32(payload, ref i);
            runner.OriginY = ReadInt32(payload, ref i);
            runner.OriginZ = ReadInt32(payload, ref i);
            runner.EndX = ReadInt32(payload, ref i);
            runner.EndY = ReadInt32(payload, ref i);
            runner.EndZ = ReadInt32(payload, ref i);
            runner.CollisionHeight = BitConverter.Int64BitsToDouble(ReadInt64(payload, ref i));
            runner.CollisionRadius = BitConverter.Int64BitsToDouble(ReadInt64(payload, ref i));
            runner.Unknown120 = ReadInt32(payload, ref i);
            runner.Speeds = new byte[20];
            for (int s = 0; s < 20; s++)
                runner.Speeds[s] = ReadByte(payload, ref i);
            ReadInt32(payload, ref i);
            runners[r] = runner;
        }

        return new MonRaceInfoNetMsg { CodeA = codeA, CodeB = codeB, Runners = runners };
    }

    static int ReadInt32(ReadOnlySpan<byte> payload, ref int i)
    {
        int v = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(i, 4));
        i += 4;
        return v;
    }

    static long ReadInt64(ReadOnlySpan<byte> payload, ref int i)
    {
        long v = BinaryPrimitives.ReadInt64LittleEndian(payload.Slice(i, 8));
        i += 8;
        return v;
    }

    static byte ReadByte(ReadOnlySpan<byte> payload, ref int i)
    {
        byte v = payload[i];
        i += 1;
        return v;
    }
}