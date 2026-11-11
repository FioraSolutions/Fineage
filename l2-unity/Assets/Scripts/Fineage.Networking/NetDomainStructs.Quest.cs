using System;


/// <summary>
/// Fineage Net payload rows shared by the Net domain mapping.
/// Engine-free: no UnityEngine references.
/// </summary>

public struct NetIdCount
{
    public int Id;
    public int Count;
}

public struct NetNpcHtml
{
    public int NpcObjectId;
    public int NpcId;
    public string Html;
    public int ItemId;
}

public struct NetQuestJournal
{
    public int QuestId;
    public string Name;
    public int State;
    public int Cond;
    public int ProgressFlags;
    public int JournalWord;
    public bool Repeatable;
}

public struct NetQuestList
{
    public NetQuestJournal[] Active;
    public byte[] CompletedMask;
    public int[] CompletedQuestIds;
    public int MaxActive;
}

public struct NetQuestDialog
{
    public int NpcObjectId;
    public int NpcId;
    public int QuestId;
    public string Hook;
    public string Event;
    public string Html;
}
