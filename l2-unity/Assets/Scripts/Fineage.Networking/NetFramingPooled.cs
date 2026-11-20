using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Fineage.Net.Contract;

/// <summary>
/// Pooled TCP frame reader (client-only; keep in sync with backend NetFraming when porting).
/// </summary>
public static class NetFramingPooled
{
    public static async Task<PacketEnvelope?> ReadAsync(Stream stream, byte[] lengthScratch, CancellationToken ct)
    {
        if (lengthScratch == null || lengthScratch.Length < 4)
            throw new ArgumentException("lengthScratch must hold at least 4 bytes", nameof(lengthScratch));

        if (!await ReadExactAsync(stream, lengthScratch, 4, ct).ConfigureAwait(false))
            return null;

        uint total = BinaryPrimitives.ReadUInt32LittleEndian(lengthScratch.AsSpan(0, 4));
        if (total < PacketEnvelope.HeaderSize || total > PacketEnvelope.MaxFrameBytes)
            throw new InvalidOperationException("bad_frame_length");

        byte[] rented = ArrayPool<byte>.Shared.Rent((int)total);
        try
        {
            if (!await ReadExactAsync(stream, rented, (int)total, ct).ConfigureAwait(false))
                return null;

            if (!NetFraming.TryDecode(rented.AsSpan(0, (int)total), out var envelope, out string error))
                throw new InvalidOperationException(error ?? "decode");
            return envelope;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    static async Task<bool> ReadExactAsync(Stream stream, byte[] buffer, int length, CancellationToken ct)
    {
        int offset = 0;
        while (offset < length)
        {
            int n = await stream.ReadAsync(buffer.AsMemory(offset, length - offset), ct).ConfigureAwait(false);
            if (n == 0)
                return false;
            offset += n;
        }

        return true;
    }
}