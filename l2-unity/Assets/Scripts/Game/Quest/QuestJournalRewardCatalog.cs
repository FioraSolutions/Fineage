using System.Collections.Generic;
using Fineage.Net.Contract;

public static class QuestJournalRewardCatalog
{
    static readonly Dictionary<int, QuestJournalRewardNet[]> _byQuest = new();

    public static void Apply(QuestListNetMsg list)
    {
        _byQuest.Clear();
        var sets = list?.RewardSets;
        if (sets == null)
            return;
        for (int i = 0; i < sets.Length; i++)
        {
            var set = sets[i];
            if (set == null || set.QuestId <= 0)
                continue;
            _byQuest[set.QuestId] = set.Rows ?? System.Array.Empty<QuestJournalRewardNet>();
        }
    }

    public static IReadOnlyList<QuestJournalRewardNet> RowsFor(int questId) =>
        _byQuest.TryGetValue(questId, out var rows) ? rows : System.Array.Empty<QuestJournalRewardNet>();
}