using System;

/// <summary>
/// Login → Confirm → AuthLogin → char-select breadcrumb log.
/// Console filter: <c>[LOBBY]</c>. File: <c>logs/diag/lobby-flow.txt</c> (rotated).
/// Active only while GameState &lt; IN_GAME so world spam stays out.
/// Facade over <see cref="FineageLog"/> (Lobby category).
/// Stays cycle-safe inside <c>Fineage.Networking</c>: Assembly-CSharp wires the active gate.
/// </summary>
public static class LobbyFlowLog
{
    public const string Tag = "[LOBBY]";

    static Func<bool> _isActive;

    /// <summary>
    /// Optional lobby-active predicate from Assembly-CSharp
    /// (typically <c>manager == null || manager.GameState &lt; GameState.IN_GAME</c>).
    /// When unset, lobby logging stays on.
    /// </summary>
    public static void Wire(Func<bool> isActive = null)
    {
        _isActive = isActive;
    }

    public static bool Active => _isActive == null || _isActive();

    public static void Info(string message)
    {
        if (!Active)
            return;
        FineageLog.Info(FineageLogCategory.Lobby, message);
    }

    public static void Warn(string message)
    {
        FineageLog.Warn(FineageLogCategory.Lobby, message);
    }

    public static void Error(string message)
    {
        FineageLog.Error(FineageLogCategory.Lobby, message);
    }

    public static void Exception(string where, Exception ex)
    {
        FineageLog.Exception(FineageLogCategory.Lobby, where, ex);
    }

    public static string OpcodeName(byte opcode)
    {
        if (Enum.IsDefined(typeof(GameServerPacketType), opcode))
            return ((GameServerPacketType)opcode).ToString();
        return "UNKNOWN";
    }
}
