using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

/// <summary>Resolves active quest rows against Interlude questname data. Logs concrete errors — no silent fallbacks.</summary>
public static class QuestJournalResolver
{
    public const string LogTag = "[QuestJournal]";

    public static int StepProg(QuestInstance quest)
    {
        if (quest == null)
            return 0;
        int prog = quest.StepProg;
        return prog > 0 ? prog : 1;
    }

    public static QuestName ResolveStep(QuestInstance quest)
    {
        if (quest == null)
            return null;

        QuestNameTable table = TablePacketServices.QuestNames;
        if (table == null)
        {
            Debug.LogError($"{LogTag} quest id={quest.QuestID} step=? — QuestNameTable is not initialized.");
            return null;
        }

        int step = StepProg(quest);
        QuestName row = table.GetQuestName(quest.QuestID, step);
        if (row == null)
        {
            Debug.LogError($"{LogTag} missing questname row quest id={quest.QuestID} step={step} (cond={quest.Cond} journalWord={quest.JournalWord}).");
        }

        return row;
    }

    public static string StepDescription(QuestInstance quest)
    {
        QuestName row = ResolveStep(quest);
        if (row == null)
            return "";
        if (!string.IsNullOrEmpty(row.Description))
            return row.Description;
        Debug.LogError($"{LogTag} empty description quest id={quest.QuestID} step={StepProg(quest)}.");
        return "";
    }

    public static string StepLocation(QuestInstance quest)
    {
        QuestName row = ResolveStep(quest);
        if (row == null)
            return "";

        var parts = new List<string>(2);
        if (!string.IsNullOrEmpty(row.EntityName))
            parts.Add(row.EntityName.Trim());
        if (row.ContactNpcId > 0)
            parts.Add($"NPC {row.ContactNpcId}");
        if (row.HasWorldTarget)
            parts.Add($"({row.QuestWorldX}, {row.QuestWorldY})");
        return parts.Count > 0 ? string.Join(" — ", parts) : "";
    }

    public static string FormatItemCounts(QuestInstance quest, Func<int, int> inventoryCount)
    {
        QuestName row = ResolveStep(quest);
        if (row?.RequiredItemCounts == null || row.RequiredItemCounts.Count == 0)
            return "";

        inventoryCount ??= _ => 0;
        var sb = new StringBuilder();
        foreach (QuestStepItemCount req in row.RequiredItemCounts)
        {
            if (req.ItemId <= 0 || req.Required <= 0)
                continue;
            int have = inventoryCount(req.ItemId);
            if (sb.Length > 0)
                sb.Append('\n');
            sb.Append($"Item {req.ItemId}: {have} / {req.Required}");
        }

        return sb.ToString();
    }

    public static int CountInBag(int itemId)
    {
        PlayerInventory bag = InventoryPacketServices.Inventory;
        if (bag == null || itemId <= 0)
            return 0;
        int total = 0;
        foreach (ItemInstance row in bag.GetPlayerInvetoryToList())
        {
            if (row != null && row.ItemId == itemId)
                total += row.Count;
        }

        return total;
    }

    public static QuestName ResolveCompletedTitle(int questId)
    {
        QuestNameTable table = TablePacketServices.QuestNames;
        if (table == null)
            return null;

        List<QuestName> rows = table.GetQuestNames(questId);
        if (rows == null || rows.Count == 0)
        {
            Debug.LogError($"{LogTag} missing questname for completed quest id={questId}.");
            return null;
        }

        return rows[rows.Count - 1];
    }

    public static List<QuestName> GetQuestNames(int questId)
    {
        QuestNameTable table = TablePacketServices.QuestNames;
        return table != null ? table.GetQuestNames(questId) : null;
    }
}