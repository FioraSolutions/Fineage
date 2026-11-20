/// <summary>
/// Interlude QuestWnd tab assignment. quest_type (questname-e col 42) is repeatability:
/// 0/2 multi-use, 1/3 one-time (QuestListWnd.uc). Epic/transfer buckets use Mobius quest id bands.
/// </summary>
public enum QuestWndTab
{
    Single,
    Repeat,
    Epic,
    Transfer,
    Special,
    Completed,
}

public static class QuestWndTabFilter
{
    public static bool IsEpicQuest(int questId) =>
        questId is >= 70 and <= 100 or >= 211 and <= 247 or >= 334 and <= 387;

    public static bool IsTransferQuest(int questId) =>
        questId is >= 401 and <= 418;

    public static bool IsSpecialQuest(int questId, int questType, int clanPetQuest) =>
        questType is 0 or 1
        || clanPetQuest != 0
        || questId is 255 or 999 or 256 or 27 or 28 or 29 or 30 or 50 or 51 or 52 or 53 or 426
        || questId is >= 419 and <= 421
        || questId is >= 501 and <= 510
        || questId is 11 or 12
        || questId is >= 605 and <= 616;

    public static bool IsRepeatQuestType(int questType) => questType is 0 or 2;

    public static QuestWndTab TabForActive(int questId, int questType, int clanPetQuest, bool serverRepeatable)
    {
        if (IsEpicQuest(questId))
            return QuestWndTab.Epic;
        if (IsTransferQuest(questId))
            return QuestWndTab.Transfer;
        if (IsSpecialQuest(questId, questType, clanPetQuest))
            return QuestWndTab.Special;
        if (serverRepeatable || IsRepeatQuestType(questType))
            return QuestWndTab.Repeat;
        return QuestWndTab.Single;
    }
}