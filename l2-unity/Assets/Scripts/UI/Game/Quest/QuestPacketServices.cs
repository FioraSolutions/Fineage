/// <summary>
/// Quest UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 17).
/// </summary>
public static class QuestPacketServices
{
    private static GameClient _game;
    private static QuestWindow _quest;

    public static void Bind(GameClient game = null, QuestWindow quest = null)
    {
        if (game != null)
            _game = game;
        if (quest != null)
            _quest = quest;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static QuestWindow Quest =>
        _quest != null ? _quest : IncomingPacketActions.Quest;
}
