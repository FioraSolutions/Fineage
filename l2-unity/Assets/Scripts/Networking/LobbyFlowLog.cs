using System;

/// <summary>
/// Login → Confirm → AuthLogin → char-select breadcrumb log.
/// Console filter: <c>[LOBBY]</c>. File: <c>logs/diag/lobby-flow.txt</c> (rotated).
/// Active only while GameState &lt; IN_GAME so world spam stays out.
/// Facade over <see cref="FineageLog"/> (Lobby category).
/// </summary>
public static class LobbyFlowLog
{
    public const string Tag = "[LOBBY]";
    static GameManager _manager;

    public static void Wire(GameManager manager)
    {
        if (manager != null)
            _manager = manager;
    }

    static GameManager Manager
    {
        get { return _manager != null ? _manager : IncomingPacketActions.Manager; }
    }

    public static bool Active
    {
        get
        {
            GameManager manager = Manager;
            return manager == null || manager.GameState < GameState.IN_GAME;
        }
    }

    public static void Info(string message)
    {
        if (!Active)
            return;
        FineageLog.Info(FineageLog.Category.Lobby, message);
    }

    public static void Warn(string message)
    {
        FineageLog.Warn(FineageLog.Category.Lobby, message);
    }

    public static void Error(string message)
    {
        FineageLog.Error(FineageLog.Category.Lobby, message);
    }

    public static void Exception(string where, Exception ex)
    {
        FineageLog.Exception(FineageLog.Category.Lobby, where, ex);
    }

    public static string OpcodeName(byte opcode)
    {
        if (Enum.IsDefined(typeof(GameServerPacketType), opcode))
            return ((GameServerPacketType)opcode).ToString();
        return "UNKNOWN";
    }
}
