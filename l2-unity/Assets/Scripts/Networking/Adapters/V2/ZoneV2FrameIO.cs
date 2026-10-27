using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Fineage v2 frame: LE ushort totalLength (incl. header) | LE ushort msgType | MessagePack body.
/// </summary>
public static class ZoneV2FrameIO
{
    public const int HeaderSize = 4;

    public static async Task SendAsync(NetworkStream stream, ZoneV2MsgType type, byte[] body, CancellationToken ct)
    {
        if (body == null)
            body = Array.Empty<byte>();
        int total = HeaderSize + body.Length;
        if (total > ushort.MaxValue)
            throw new InvalidOperationException("frame too large: " + total);

        byte[] packet = new byte[total];
        packet[0] = (byte)(total & 0xff);
        packet[1] = (byte)((total >> 8) & 0xff);
        ushort t = (ushort)type;
        packet[2] = (byte)(t & 0xff);
        packet[3] = (byte)((t >> 8) & 0xff);
        if (body.Length > 0)
            Buffer.BlockCopy(body, 0, packet, HeaderSize, body.Length);

        await stream.WriteAsync(packet, 0, packet.Length, ct).ConfigureAwait(false);
        await stream.FlushAsync(ct).ConfigureAwait(false);
    }

    public static async Task<ZoneV2Frame?> TryReadAsync(NetworkStream stream, CancellationToken ct)
    {
        byte[] header = new byte[HeaderSize];
        if (!await ReadExactAsync(stream, header, ct).ConfigureAwait(false))
            return null;

        ushort total = (ushort)(header[0] | (header[1] << 8));
        ushort type = (ushort)(header[2] | (header[3] << 8));
        if (total < HeaderSize)
            return null;

        int bodyLen = total - HeaderSize;
        byte[] body = bodyLen == 0 ? Array.Empty<byte>() : new byte[bodyLen];
        if (bodyLen > 0 && !await ReadExactAsync(stream, body, ct).ConfigureAwait(false))
            return null;

        return new ZoneV2Frame((ZoneV2MsgType)type, body);
    }

    static async Task<bool> ReadExactAsync(NetworkStream stream, byte[] buffer, CancellationToken ct)
    {
        int off = 0;
        while (off < buffer.Length)
        {
            int n = await stream.ReadAsync(buffer, off, buffer.Length - off, ct).ConfigureAwait(false);
            if (n == 0)
                return false;
            off += n;
        }

        return true;
    }
}

public readonly struct ZoneV2Frame
{
    public readonly ZoneV2MsgType Type;
    public readonly byte[] Body;

    public ZoneV2Frame(ZoneV2MsgType type, byte[] body)
    {
        Type = type;
        Body = body ?? Array.Empty<byte>();
    }
}
