using System;
using Fineage.Net.Contract;

/// <summary>Client-side MonRaceInfo decode until Backend contract adds <see cref="NetCodec.DecodeMonRaceInfo"/>.</summary>
public static partial class NetZonePacketDecoder
{
    static MonRaceInfoNetMsg DecodeMonRaceInfo(ReadOnlySpan<byte> payload)
    {
        int i = 0;
        int ReadInt32()
        {
            int v = System.Buffers.Binary.BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(i, 4));
            i += 4;
            return v;
        }

        byte ReadByte()
        {
            byte v = payload[i];
            i += 1;
            return v;
        }

        int codeA = ReadInt32();
        int codeB = ReadInt32();
        int n = ReadInt32();
        if (n < 0)
            n = 0;
        var runners = new MonRaceRunnerNetMsg[n];
        for (int r = 0; r < n; r++)
        {
            var runner = new MonRaceRunnerNetMsg { ObjectId = ReadInt32() };
            runner.NpcIdPlusDisplay = ReadInt32();
            runner.OriginX = ReadInt32();
            runner.OriginY = ReadInt32();
            runner.OriginZ = ReadInt32();
            runner.EndX = ReadInt32();
            runner.EndY = ReadInt32();
            runner.EndZ = ReadInt32();
            runner.CollisionHeight = BitConverter.Int64BitsToDouble(
                System.Buffers.Binary.BinaryPrimitives.ReadInt64LittleEndian(payload.Slice(i, 8)));
            i += 8;
            runner.CollisionRadius = BitConverter.Int64BitsToDouble(
                System.Buffers.Binary.BinaryPrimitives.ReadInt64LittleEndian(payload.Slice(i, 8)));
            i += 8;
            runner.Unknown120 = ReadInt32();
            runner.Speeds = new byte[20];
            for (int s = 0; s < 20; s++)
                runner.Speeds[s] = ReadByte();
            ReadInt32();
            runners[r] = runner;
        }

        return new MonRaceInfoNetMsg { CodeA = codeA, CodeB = codeB, Runners = runners };
    }
}