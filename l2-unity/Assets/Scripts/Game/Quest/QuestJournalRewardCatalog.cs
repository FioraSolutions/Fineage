using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static class QuestJournalRewardCatalog
{
    static readonly Dictionary<int, QuestJournalRewardNet[]> _byQuest = new();
    static readonly HashSet<int> _missingQuestLogged = new();

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

    public static IReadOnlyList<QuestJournalRewardNet> RowsFor(int questId)
    {
        if (_byQuest.TryGetValue(questId, out var rows))
            return rows;

        if (questId > 0 && _missingQuestLogged.Add(questId))
        {
            Debug.LogError(
                QuestJournalResolver.LogTag + " missing reward rows quest id=" + questId
                + " (zone QuestListNetMsg had no RewardSets entry).");
        }

        return System.Array.Empty<QuestJournalRewardNet>();
    }
}