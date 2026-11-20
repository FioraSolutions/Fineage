/// <summary>Interlude overhead quest markers (Mobius quest mark kinds).</summary>
public static class QuestNpcMarkKind
{
    public const byte None = 0;
    /// <summary>Blue question — quest available from NPC.</summary>
    public const byte Available = 1;
    /// <summary>Yellow exclamation — quest in progress at NPC.</summary>
    public const byte InProgress = 2;
    /// <summary>Yellow question — return to NPC to complete.</summary>
    public const byte Return = 3;
}