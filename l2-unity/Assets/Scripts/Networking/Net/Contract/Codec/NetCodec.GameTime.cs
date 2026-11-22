using System;

namespace Fineage.Net.Contract
{
    public static partial class NetCodec
    {
        /// <summary>Mobius <c>ClientSetTime</c> field order: game minutes, clock speed.</summary>
        public static byte[] EncodeClientSetTime(ClientSetTimeNetMsg m) => Write(w =>
        {
            w.WriteInt32(m.GameMinutes);
            w.WriteInt32(m.ClockSpeed);
        });

        public static ClientSetTimeNetMsg DecodeClientSetTime(ReadOnlySpan<byte> p)
        {
            var r = new Reader(p);
            return new ClientSetTimeNetMsg
            {
                GameMinutes = r.ReadInt32(),
                ClockSpeed = r.ReadInt32()
            };
        }
    }
}