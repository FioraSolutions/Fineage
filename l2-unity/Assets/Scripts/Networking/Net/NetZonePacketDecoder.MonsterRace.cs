using System;
using System.Buffers.Binary;
using Fineage.Net.Contract;

/// <summary>Client-side MonRaceInfo decode until Backend contract adds <see cref="NetCodec.DecodeMonRaceInfo"/>.</summary>
public static partial class NetZonePacketDecoder
{
    static MonRaceInfoNetMsg DecodeMonRaceInfo(ReadOnlySpan<byte> payload)
    {
        int i = 0;
        int codeA = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(i, 4));
        i += 4;
        int codeB = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(i, 4));
        i += 4;
        int n = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(i, 4));
        i += 4;
        if (n < 0)
            n = 0;
        var runners = new MonRaceRunnerNetMsg[n];
        for (int r = 0; r < n; r++)
        {
            var runner = new MonRaceRunnerNetMsg
            {
                ObjectId = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(i, 4)),
            };
            i += 4;
            runner.NpcIdPlusDisplay = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(i, 4));
            i += 4;
            runner.OriginX = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(i, 4));
            i += 4;
            runner.OriginY = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(i, 4));
            i += 4;
            runner.OriginZ = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(i, 4));
            i += 4;
            runner.EndX = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(i, 4));
            i += 4;
            runner.EndY = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(i, 4));
            i += 4;
            runner.EndZ = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(i, 4));
            i += 4;
            runner.CollisionHeight = BitConverter.Int64BitsToDouble(
                BinaryPrimitives.ReadInt64LittleEndian(payload.Slice(i, 8)));
            i += 8;
            runner.CollisionRadius = BitConverter.Int64BitsToDouble(
                BinaryPrimitives.ReadInt64LittleEndian(payload.Slice(i, 8)));
            i += 8;
            runner.Unknown120 = BinaryPrimitives.ReadInt32LittleEndian(payload.Slice(i, 4));
            i += 4;
            runner.Speeds = new byte[20];
            for (int s = 0; s < 20; s++)
            {
                runner.Speeds[s] = payload[i];
                i += 1;
            }

            i += 4;
            runners[r] = runner;
        }

        return new MonRaceInfoNetMsg { CodeA = codeA, CodeB = codeB, Runners = runners };
    }
}