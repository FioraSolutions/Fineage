using Fineage.Net.Contract;

public class QuestInstance
{
    private int _questID;
    private int _flags;
    private int _repeat;
    private string _wireName = "";
    private int _state;
    private int _cond;
    private int _journalWord;
    private int _progressFlags;

    public int QuestID => _questID;
    public int Status => _flags;
    public int State => _state;
    public int Cond => _cond;
    public int JournalWord => _journalWord;
    public int ProgressFlags => _progressFlags;

    /// <summary>Interlude journal step id (JournalWord, else Cond).</summary>
    public int StepProg
    {
        get
        {
            if (_journalWord != 0) return _journalWord;
            if (_cond != 0) return _cond;
            return _flags;
        }
    }

    public QuestInstance(int qId, int flags)
    {
        _questID = qId;
        _flags = flags;
        _repeat = -1;
    }

    public QuestInstance(int qId, int flags , int repeat)
    {
        _questID = qId;
        _flags = flags;
        _repeat = repeat;
    }

    public QuestInstance(int qId, int flags, int repeat, string wireName)
    {
        _questID = qId;
        _flags = flags;
        _repeat = repeat;
        _wireName = wireName ?? "";
    }

    public static QuestInstance CompletedTitleRow(int questId, string title)
    {
        var inst = new QuestInstance(questId, 0, 0, title);
        inst._state = 2;
        return inst;
    }

    public static QuestInstance FromNet(QuestJournalEntryNet row)
    {
        int flags = row.JournalWord != 0 ? row.JournalWord : row.Cond;
        if (flags == 0)
            flags = 1;
        var inst = new QuestInstance(row.QuestId, flags, row.Repeatable ? 1 : 0, row.Name ?? "")
        {
            _state = row.State,
            _cond = row.Cond,
            _journalWord = row.JournalWord,
            _progressFlags = row.ProgressFlags
        };
        return inst;
    }

    /// <summary>Quest is completed in the journal (not the same as “active row with cond 0”).</summary>
    public bool IsComplete() => _state == 2;

    private QuestName GetQuestName() => QuestJournalResolver.ResolveStep(this);

    //public bool IsRepeat() => GetQuestName()?.Repeat == 1;
    public bool IsRepeat()
    {
        if (_repeat != -1) return _repeat == 1;
        return GetQuestName()?.Repeat == 1;
    }

    public string QuestName()
    {
        if (!string.IsNullOrEmpty(_wireName))
            return _wireName;
        return GetQuestName()?.Main_name ?? "";
    }

    public string LevelRange() => GetQuestName()?.GetLevelRange() ?? "";

    public string QuestProgName() => GetQuestName()?.ProgName ?? "";

    public string GetQuestSource() => GetQuestName()?.GetSource() ?? "";

    public string GetQuestEntity() => GetQuestName()?.EntityName ?? "";

    public string GetDescription() => QuestJournalResolver.StepDescription(this);

    public string GetLocationLine() => QuestJournalResolver.StepLocation(this);

    public string GetItemCountLine() =>
        QuestJournalResolver.FormatItemCounts(this, QuestJournalResolver.CountInBag);
}