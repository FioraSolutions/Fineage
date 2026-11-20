using Fineage.Net.Contract;

/// <summary>Decoded zone packet: apply on the main thread only; body may be merged while pending.</summary>
public sealed class ZoneApplyPacket
{
    public ushort Opcode { get; set; }
    /// <summary>Null for opcodes with no decoded body (e.g. HelloAck).</summary>
    public object Body { get; set; }
    public PacketEnvelope Envelope { get; set; }
}