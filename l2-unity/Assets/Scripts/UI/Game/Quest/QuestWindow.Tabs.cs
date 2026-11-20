using System.Collections.Generic;
using System.Linq;

public partial class QuestWindow
{
    private const string _completedTabName = "Completed";

    private IContent _questTabPanelEpic;
    private IContent _questTabPanelTransfer;
    private IContent _questTabPanelSpecial;
    private IContent _questTabPanelCompleted;

    private List<QuestInstance> _tabSingle = new();
    private List<QuestInstance> _tabRepeat = new();
    private List<QuestInstance> _tabEpic = new();
    private List<QuestInstance> _tabTransfer = new();
    private List<QuestInstance> _tabSpecial = new();
    private List<QuestInstance> _tabCompleted = new();

    void PartitionActiveQuests(List<QuestInstance> quests)
    {
        _tabSingle.Clear();
        _tabRepeat.Clear();
        _tabEpic.Clear();
        _tabTransfer.Clear();
        _tabSpecial.Clear();
        foreach (QuestInstance quest in quests)
        {
            QuestName meta = QuestJournalResolver.ResolveStep(quest) ?? QuestJournalResolver.ResolveCompletedTitle(quest.QuestID);
            int questType = meta?.QuestType ?? 0;
            int clanPet = meta?.ClanPetQuest ?? 0;
            switch (QuestWndTabFilter.TabForActive(quest.QuestID, questType, clanPet, quest.IsRepeat()))
            {
                case QuestWndTab.Epic:
                    _tabEpic.Add(quest);
                    break;
                case QuestWndTab.Transfer:
                    _tabTransfer.Add(quest);
                    break;
                case QuestWndTab.Special:
                    _tabSpecial.Add(quest);
                    break;
                case QuestWndTab.Repeat:
                    _tabRepeat.Add(quest);
                    break;
                default:
                    _tabSingle.Add(quest);
                    break;
            }
        }
    }

    void RefreshCompletedTab()
    {
        _tabCompleted.Clear();
        foreach (int questId in QuestJournalState.Completed)
        {
            QuestName meta = QuestJournalResolver.ResolveCompletedTitle(questId);
            string title = meta?.Main_name;
            if (string.IsNullOrEmpty(title))
                title = $"Quest {questId}";
            _tabCompleted.Add(QuestInstance.CompletedTitleRow(questId, title));
        }
    }

    List<QuestInstance> TabList(string tabName) => tabName switch
    {
        _singleTabName => _tabSingle,
        _repeatTabName => _tabRepeat,
        _epicTabName => _tabEpic,
        _transferTabName => _tabTransfer,
        _specialTabName => _tabSpecial,
        _completedTabName => _tabCompleted,
        _ => _tabSingle
    };

    IContent TabPanel(string tabName) => tabName switch
    {
        _singleTabName => _questTabPanelSingle,
        _repeatTabName => _questTabPanelRepeat,
        _epicTabName => _questTabPanelEpic,
        _transferTabName => _questTabPanelTransfer,
        _specialTabName => _questTabPanelSpecial,
        _completedTabName => _questTabPanelCompleted,
        _ => _questTabPanelSingle
    };

    void BindRewardPanel(QuestInstance quest)
    {
        _questsList.Clear();
        if (quest == null)
        {
            _creatorRewardTables.AddOtherData(_questsList);
            return;
        }

        var rows = QuestJournalRewardCatalog.RowsFor(quest.QuestID);
        if (rows.Count == 0)
        {
            UnityEngine.Debug.LogError($"[QuestJournal] no reward data quest id={quest.QuestID}");
            _creatorRewardTables.AddOtherData(_questsList);
            return;
        }

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            if (row == null)
                continue;
            string icon = row.Kind switch
            {
                0 => ResolveItemIcon(row.ItemId),
                1 => "etc_adena_i00",
                2 => "etc_exp_point_i00",
                3 => "etc_sp_point_i00",
                _ => "etc_question_mark_i00"
            };
            string amount = row.Count.ToString();
            _questsList.Add(new OtherModel(new ModelQuestJournalReward(row.Label ?? "", amount, icon)));
        }
        _creatorRewardTables.AddOtherData(_questsList);
    }

    static string ResolveItemIcon(int itemId)
    {
        AbstractItem item = TablePacketServices.Items?.GetItem(itemId);
        string icon = item?.Icon;
        if (string.IsNullOrEmpty(icon))
            UnityEngine.Debug.LogError($"[QuestJournal] missing item icon for reward itemId={itemId}");
        return string.IsNullOrEmpty(icon) ? "etc_question_mark_i00" : icon;
    }
}