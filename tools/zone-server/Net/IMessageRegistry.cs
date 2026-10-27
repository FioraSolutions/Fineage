namespace Fineage.ZoneServer.Net;

/// <summary>
/// Maps message opcodes to CLR types and provides protocol version gating.
/// Used for dispatch (incoming) and for future codec negotiation.
/// </summary>
public interface IMessageRegistry
{
    /// <summary>Register a message type for a given opcode, available from the specified protocol version.</summary>
    void Register<T>(ushort msgType, int minProtocolVersion = 1) where T : class;

    /// <summary>Resolve CLR type for an opcode, respecting the negotiated protocol version.</summary>
    Type? Resolve(ushort msgType, int protocolVersion);

    /// <summary>Check if an opcode is known (any version).</summary>
    bool IsKnown(ushort msgType);
}
