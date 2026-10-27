using System.Collections.Concurrent;

namespace Fineage.ZoneServer.Net;

/// <summary>
/// Default message registry. Thread-safe for registration at startup
/// and concurrent resolution at runtime.
/// </summary>
public sealed class MessageRegistry : IMessageRegistry
{
    readonly record struct Entry(Type ClrType, int MinProtocolVersion);

    readonly ConcurrentDictionary<ushort, Entry> _map = new();

    public void Register<T>(ushort msgType, int minProtocolVersion = 1) where T : class
    {
        _map[msgType] = new Entry(typeof(T), minProtocolVersion);
    }

    public Type? Resolve(ushort msgType, int protocolVersion)
    {
        if (!_map.TryGetValue(msgType, out var entry))
            return null;
        if (protocolVersion < entry.MinProtocolVersion)
            return null;
        return entry.ClrType;
    }

    public bool IsKnown(ushort msgType) => _map.ContainsKey(msgType);

    /// <summary>
    /// Pre-populate with all current v2 wire messages.
    /// Called once at server startup.
    /// </summary>
    public static MessageRegistry CreateDefault()
    {
        var reg = new MessageRegistry();

        // C → S
        reg.Register<Wire.HelloMsg>((ushort)Wire.MsgType.Hello);
        reg.Register<Wire.AuthMsg>((ushort)Wire.MsgType.Auth);
        reg.Register<Wire.EnterWorldMsg>((ushort)Wire.MsgType.EnterWorld);
        reg.Register<Wire.MoveToMsg>((ushort)Wire.MsgType.MoveTo);

        // S → C
        reg.Register<Wire.HelloAckMsg>((ushort)Wire.MsgType.HelloAck);
        reg.Register<Wire.AuthOkMsg>((ushort)Wire.MsgType.AuthOk);
        reg.Register<Wire.PlayerSpawnedMsg>((ushort)Wire.MsgType.PlayerSpawned);
        reg.Register<Wire.MoveToMsg>((ushort)Wire.MsgType.MoveToAck);
        reg.Register<Wire.ErrorMsg>((ushort)Wire.MsgType.Error);

        return reg;
    }
}
