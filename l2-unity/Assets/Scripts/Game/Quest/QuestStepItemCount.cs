public readonly struct QuestStepItemCount
{
    public QuestStepItemCount(int itemId, int required)
    {
        ItemId = itemId;
        Required = required;
    }

    public int ItemId { get; }
    public int Required { get; }
}