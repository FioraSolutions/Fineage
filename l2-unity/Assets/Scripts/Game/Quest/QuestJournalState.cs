using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

/// <summary>Player quest journal mirror + tracked quest for the shared HUD direction marker.</summary>
public static class QuestJournalState
{
    static readonly List<QuestInstance> _active = new();
    static readonly List<int> _completed = new();
    static QuestDirectionTarget _direction = QuestDirectionTarget.Hidden;
    static readonly Dictionary<int, byte> _npcMarks = new();

    public static IReadOnlyList<QuestInstance> Active => _active;
    public static IReadOnlyList<int> Completed => _completed;
    public static QuestDirectionTarget Direction => _direction;
    public static IReadOnlyDictionary<int, byte> NpcMarks => _npcMarks;

    public static int TrackedQuestId { get; set; }

    public static void ApplyQuestList(QuestListNetMsg list)
    {
        _active.Clear();
        _completed.Clear();

        if (list?.Active != null)
        {
            for (int i = 0; i < list.Active.Length; i++)
            {
                QuestJournalEntryNet row = list.Active[i];
                if (row == null || row.QuestId <= 0)
                    continue;
                _active.Add(QuestInstance.FromNet(row));
            }
        }

        int[] done = list?.CompletedQuestIds;
        if (done != null)
        {
            for (int i = 0; i < done.Length; i++)
            {
                if (done[i] > 0)
                    _completed.Add(done[i]);
            }
        }

        if (TrackedQuestId <= 0 && _active.Count > 0)
            TrackedQuestId = _active[0].QuestID;

        QuestJournalRewardCatalog.Apply(list);
    }

    public static void ApplyDirection(QuestDirectionNetMsg msg)
    {
        if (msg == null || !msg.Show)
        {
            _direction = QuestDirectionTarget.Hidden;
            return;
        }

        _direction = new QuestDirectionTarget(msg.QuestId, msg.X, msg.Y, msg.Z);
    }

    public static void ApplyNpcMarks(QuestNpcMarksNetMsg msg)
    {
        _npcMarks.Clear();
        var rows = msg?.Marks;
        if (rows == null)
            return;
        for (int i = 0; i < rows.Length; i++)
        {
            QuestNpcMarkNet row = rows[i];
            if (row.NpcId <= 0 || row.Mark == 0)
                continue;
            _npcMarks[row.NpcId] = row.Mark;
        }
    }

    public static byte NpcMarkForTemplate(int npcTemplateId) =>
        npcTemplateId > 0 && _npcMarks.TryGetValue(npcTemplateId, out byte mark) ? mark : (byte)0;

    public static QuestInstance TrackedQuest()
    {
        if (TrackedQuestId > 0)
        {
            for (int i = 0; i < _active.Count; i++)
            {
                if (_active[i].QuestID == TrackedQuestId)
                    return _active[i];
            }
        }

        return _active.Count > 0 ? _active[0] : null;
    }

    public static void RefreshDirectionFromMetadata()
    {
        QuestInstance quest = TrackedQuest();
        if (quest == null)
        {
            _direction = QuestDirectionTarget.Hidden;
            return;
        }

        QuestName row = QuestJournalResolver.ResolveStep(quest);
        if (row == null || !row.HasWorldTarget)
        {
            _direction = QuestDirectionTarget.Hidden;
            return;
        }

        _direction = new QuestDirectionTarget(quest.QuestID, row.QuestWorldX, row.QuestWorldY, row.QuestWorldZ);
    }
}

public readonly struct QuestDirectionTarget
{
    public static readonly QuestDirectionTarget Hidden = new(0, 0, 0, 0, false);

    public QuestDirectionTarget(int questId, int x, int y, int z, bool show = true)
    {
        QuestId = questId;
        X = x;
        Y = y;
        Z = z;
        Show = show;
    }

    public int QuestId { get; }
    public int X { get; }
    public int Y { get; }
    public int Z { get; }
    public bool Show { get; }
}