using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Fineage.Net.Contract
{
    /// <summary>
    /// Frame = u32 LE totalLength (header+payload) + PacketEnvelope header + payload.
    /// Network thread only: decode/validate/enqueue — never mutate world state.
    /// </summary>
    public static class NetFraming
    {
        public static byte[] Encode(PacketEnvelope envelope)
        {
            if (envelope.Payload.Length > PacketEnvelope.MaxPayloadBytes)
                throw new InvalidOperationException("payload too large");

            int total = PacketEnvelope.HeaderSize + envelope.Payload.Length;
            var frame = new byte[4 + total];
            BinaryPrimitives.WriteUInt32LittleEndian(frame.AsSpan(0, 4), (uint)total);
            int o = 4;
            frame[o++] = envelope.Major;
            frame[o++] = envelope.Minor;
            BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(o), envelope.Opcode);
            o += 2;
            BinaryPrimitives.WriteUInt16LittleEndian(frame.AsSpan(o), envelope.Flags);
            o += 2;
            BinaryPrimitives.WriteUInt32LittleEndian(frame.AsSpan(o), envelope.RequestSequence);
            o += 4;
            BinaryPrimitives.WriteUInt32LittleEndian(frame.AsSpan(o), (uint)envelope.Payload.Length);
            o += 4;
            if (envelope.Payload.Length > 0)
                Buffer.BlockCopy(envelope.Payload, 0, frame, o, envelope.Payload.Length);
            return frame;
        }

        public static bool TryDecode(ReadOnlySpan<byte> frameBody, out PacketEnvelope envelope, out string? error)
        {
            envelope = null!;
            error = null;
            if (frameBody.Length < PacketEnvelope.HeaderSize)
            {
                error = "short_header";
                return false;
            }

            byte major = frameBody[0];
            byte minor = frameBody[1];
            ushort opcode = BinaryPrimitives.ReadUInt16LittleEndian(frameBody.Slice(2));
            ushort flags = BinaryPrimitives.ReadUInt16LittleEndian(frameBody.Slice(4));
            uint seq = BinaryPrimitives.ReadUInt32LittleEndian(frameBody.Slice(6));
            uint payloadLen = BinaryPrimitives.ReadUInt32LittleEndian(frameBody.Slice(10));
            if (payloadLen > PacketEnvelope.MaxPayloadBytes)
            {
                error = "oversized";
                return false;
            }

            if (frameBody.Length != PacketEnvelope.HeaderSize + (int)payloadLen)
            {
                error = "length_mismatch";
                return false;
            }

            var payload = payloadLen == 0
                ? Array.Empty<byte>()
                : frameBody.Slice(PacketEnvelope.HeaderSize, (int)payloadLen).ToArray();

            envelope = new PacketEnvelope
            {
                Major = major,
                Minor = minor,
                Opcode = opcode,
                Flags = flags,
                RequestSequence = seq,
                Payload = payload
            };
            return true;
        }

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

                if (!TryDecode(rented.AsSpan(0, (int)total), out var envelope, out string? error))
                    throw new InvalidOperationException(error ?? "decode");
                return envelope;
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(rented);
            }
        }

        public static async Task<PacketEnvelope?> ReadAsync(Stream stream, CancellationToken ct)
        {
            var lenBuf = new byte[4];
            return await ReadAsync(stream, lenBuf, ct).ConfigureAwait(false);
        }

        public static async Task WriteAsync(Stream stream, PacketEnvelope envelope, CancellationToken ct)
        {
            byte[] frame = Encode(envelope);
            await stream.WriteAsync(frame, ct).ConfigureAwait(false);
            await stream.FlushAsync(ct).ConfigureAwait(false);
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
}
