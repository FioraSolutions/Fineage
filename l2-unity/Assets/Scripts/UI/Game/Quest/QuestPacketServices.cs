/// <summary>
/// Quest UI access to game services with [Inject] wiring and live fallback (#142).
/// </summary>
public static class QuestPacketServices
{
    private static QuestWindow _quest;

    public static void Bind(QuestWindow quest = null)
    {
        if (quest != null)
            _quest = quest;
    }

    public static QuestWindow Quest =>
        _quest != null ? _quest : QuestWindow.Instance;
}
