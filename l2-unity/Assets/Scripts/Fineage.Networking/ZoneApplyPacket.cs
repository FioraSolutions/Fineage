using Fineage.Net.Contract;

/// <summary>Immutable decoded zone packet: apply on the main thread only.</summary>
public sealed class ZoneApplyPacket
{
    public ushort Opcode { get; init; }
    /// <summary>Null for opcodes with no decoded body (e.g. HelloAck).</summary>
    public object Body { get; init; }
    public PacketEnvelope Envelope { get; init; }
}